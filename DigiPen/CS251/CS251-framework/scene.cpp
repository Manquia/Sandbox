//////////////////////////////////////////////////////////////////////
// Defines and draws a scene.  There are two main procedures here:
//
// Copyright 2013 DigiPen Institute of Technology
////////////////////////////////////////////////////////////////////////

#include "math.h"
#include <fstream>
#include <stdlib.h>

#include <glbinding/gl/gl.h>
#include <glbinding/Binding.h>
using namespace gl;

#include <freeglut.h>
#include <glu.h>                // For gluErrorString

#define GLM_FORCE_RADIANS
#define GLM_SWIZZLE
#include <glm/glm.hpp>
using namespace glm;

#include "shader.h"
#include "shapes.h"
#include "scene.h"
#include "object.h"
#include "texture.h"
#include "transform.h"

const float PI = 3.14159f;
const float rad = PI/180.0f;    // Convert degrees to radians

MAT4 Identity;

const float grndSize = 100.0;    // Island radius;  Minimum about 20;  Maximum 1000 or so
const int   grndTiles = int(grndSize);
const float grndOctaves = 4.0;  // Number of levels of detail to compute
const float grndFreq = 0.03;    // Number of hills per (approx) 50m
const float grndPersistence = 0.03; // Terrain roughness: Slight:0.01  rough:0.05
const float grndLow = -3.0;         // Lowest extent below sea level
const float grndHigh = 5.0;        // Highest extent above sea level



					


// Simple procedure to print a 4x4 matrix -- useful for debugging
void PrintMat(const MAT4& m)
{
    for (int i=0;  i<4;  i++)
        for (int j=0;  j<4;  j++)
            printf("%9.4f%c", m[i][j], j==3?'\n':' ');
    printf("\n");
}

////////////////////////////////////////////////////////////////////////
// This macro makes it easy to sprinkle checks for OpenGL errors
// through out your code.  Most OpenGL calls can record errors, and a
// careful programmer will check the error status *often*, perhaps as
// often as right after every OpenGL call.  At the very least, once
// per refresh will tell you if something is going wrong.
#define CHECKERROR {GLenum err = glGetError(); if (err != GL_NO_ERROR) { fprintf(stderr, "OpenGL error (at line %d): %s\n", __LINE__, gluErrorString(err)); exit(-1);} }

Object* SphereOfSpheres(Shape* SpherePolygons)
{
    Object* ob = new Object(NULL, nullId);
    Object* sp = new Object(SpherePolygons, spheresId,
                            vec3(0.5, 0.5, 1.0), vec3(0.2, 0.2, 0.2), 120.0);
    
    for (float angle=0.0;  angle<360.0;  angle+= 18.0)
        for (float row=0.1;  row<PI/2.0;  row += PI/2.0/6.0) {
            float s = sin(row);
            float c = cos(row);
            ob->add(sp, Rotate(2,angle)*Translate(c,0,s)*Scale(0.1*c,0.1*c,0.1*c));
        }
    return ob;
}

Object* FramedPicture(const MAT4& modelTr, const int objectId, 
                      Shape* BoxPolygons, Shape* QuadPolygons)
{
    // This draws the frame as four (elongated) boxes of size +-1.0
    float w = 0.05;             // Width of frame boards.
    
    Object* frame = new Object(NULL, nullId);
    Object* ob;
    
    ob = new Object(BoxPolygons, frameId,
                    vec3(0.3, 0.3, 0.0), vec3(0.2, 0.2, 0.2), 10.0);
    frame->add(ob, Translate(0.0, 0.0, 1.0+w)*Scale(1.0, w, w));
    frame->add(ob, Translate(0.0, 0.0, -1.0-w)*Scale(1.0, w, w));
    frame->add(ob, Translate(1.0+w, 0.0, 0.0)*Scale(w, w, 1.0+2*w));
    frame->add(ob, Translate(-1.0-w, 0.0, 0.0)*Scale(w, w, 1.0+2*w));

    ob = new Object(QuadPolygons, objectId,
                    vec3(0.3, 0.2, 0.1), vec3(0.0, 0.0, 0.0), 10.0);
    frame->add(ob, Rotate(0,90));

    return frame;
}

////////////////////////////////////////////////////////////////////////
// Called regularly to update the atime global variable.
float atime = 0.0;
void animate(int value)
{
    atime = 360.0*glutGet((GLenum)GLUT_ELAPSED_TIME)/36000;
    glutPostRedisplay();

    // Schedule next call to this function
    glutTimerFunc(30, animate, 1);
}

////////////////////////////////////////////////////////////////////////
// InitializeScene is called once during setup to create all the
// textures, shape VAOs, and shader programs as well as a number of
// other parameters.
void Scene::InitializeScene()
{
    glEnable(GL_DEPTH_TEST);
    CHECKERROR;

	// Initialize Render data
	ry = 0.2f;
	frontClippingPlaneDist = 0.01f;
	backClippingPlanedist = 100000.0f;
	cameraTilt = 20.0f;
	cameraSpin = 30.0f;
	cameraZoom = 80.0f;
	cameraPan = glm::vec2(0.0f,0.0f);
	movementType = MovementType::MT_ORBIT;
	cameraPos = { 0.0f, -10.0f, 5.0f };

	time_startProgram = glutGet(static_cast<gl::GLenum>(GLUT_ELAPSED_TIME));
	time_LastFrame = time_startProgram;
	dt = 0.0167777777f;

    objectRoot = new Object(NULL, nullId);
    
    // Set the initial light position parammeters
    lightSpin = 90.0;
    lightTilt = 30.0;
    lightDist = 1000.0;

    // Enable OpenGL depth-testing
    glEnable(GL_DEPTH_TEST);

	bool RandomizeRoomPosition;
	if (movementType == Scene::MovementType::MT_ORBIT) {
		RandomizeRoomPosition = false;
	}else{
		RandomizeRoomPosition = true;
	}

    ground =  new ProceduralGround(grndSize, grndTiles, grndOctaves, grndFreq, grndPersistence, grndLow, grndHigh, RandomizeRoomPosition);

    basePoint = ground->highPoint;

    // Create the lighting shader program from source code files.
    lightingProgram = new ShaderProgram();
    lightingProgram->AddShader("lighting.vert", GL_VERTEX_SHADER);
    lightingProgram->AddShader("lighting.frag", GL_FRAGMENT_SHADER);

    glBindAttribLocation(lightingProgram->programId, 0, "vertex");
    glBindAttribLocation(lightingProgram->programId, 1, "vertexNormal");
    glBindAttribLocation(lightingProgram->programId, 2, "vertexTexture");
    glBindAttribLocation(lightingProgram->programId, 3, "vertexTangent");
    lightingProgram->LinkProgram();

    // Create all the Polygon shapes
    Shape* TeapotPolygons =  new Teapot(12);
    Shape* BoxPolygons = new Ply("box.ply");
    Shape* SpherePolygons = new Sphere(32);
    Shape* WallPolygons = new Ply("room.ply");
    Shape* GroundPolygons = ground;
    Shape* QuadPolygons = new Quad();
    Shape* SeaPolygons = new Plane(2000.0, 50);

    // Create all the models from which the scene is composed.  Each
    // is created with a polygon shape (possible NULL), a
    // transformation, and the curface lighting parameters Kd, Ks, and
    // alpha.
    Object* wall = new Object(WallPolygons, wallId,
                              vec3(0.8, 0.8, 0.5), vec3(0.0, 0.0, 0.0), 1);
    Object* rightAnim = new Object(NULL, nullId);
    Object* teapot = new Object(TeapotPolygons, teapotId,
                                vec3(0.5, 0.5, 0.1), vec3(0.2, 0.2, 0.2), 120);
    Object* rightPodium = new Object(BoxPolygons, boxId,
                                     vec3(0.25, 0.25, 0.1), vec3(0.3, 0.3, 0.3), 10);
    
    Object* leftAnim = new Object(NULL, nullId);
    Object* leftPodium = new Object(BoxPolygons, boxId,
                                    vec3(0.25, 0.25, 0.1), vec3(0.3, 0.3, 0.3), 10);

    Object* spheres = SphereOfSpheres(SpherePolygons);
    Object* leftFrame = FramedPicture(Identity, lPicId,
                                      BoxPolygons, QuadPolygons);
    Object* rightFrame = FramedPicture(Identity, rPicId,
                                      BoxPolygons, QuadPolygons);
    
    
    Object* sky = new Object(SpherePolygons, skyId,
                             vec3(), vec3(), 0);

    Object* ground = new Object(GroundPolygons, groundId,
                                vec3(0.3, 0.2, 0.1), vec3(0.0, 0.0, 0.0), 1);

    Object* sea = new Object(SeaPolygons, seaId,
                             vec3(0.3, 0.3, 1.0), vec3(0.15, 0.15, 0.15), 120);

    // FIXME: This is where you could read in all the textures and
    // associate them with the various objects just created above
    // here.

    // Scene is composed of sky, ground, sea, and wall models
    objectRoot->add(sky, Scale(2000.0, 2000.0, 2000.0));
    objectRoot->add(ground);
    objectRoot->add(wall, Translate(basePoint.x, basePoint.y, basePoint.z));
    objectRoot->add(sea);

    // Two models have rudimentary animation (constant rotation on Z)
    animated.push_back(rightAnim);
    animated.push_back(leftAnim);

    // Wall contains four assemblies: two objects on podiums, two framed pictures
    wall->add(rightPodium, Translate(2.5,0,0));
    wall->add(rightAnim, Translate(2.5,0,0));
    rightAnim->add(teapot, Translate(0.0,0,1.5)*TeapotPolygons->modelTr);
    
    wall->add(leftPodium, Translate(-2.5,0,0));
    wall->add(leftAnim, Translate(-2.5,0,0));
    leftAnim->add(spheres, Translate(0.0,0,1.0));
    
    wall->add(leftFrame, Translate(-1.5,9.85,1.)*Scale(0.8,0.8,0.8));
    wall->add(rightFrame, Translate( 1.5,9.85,1.)*Scale(0.8,0.8,0.8));

    // Schedule first timed animation call
    glutTimerFunc(30, animate, 1);

    CHECKERROR;
}



////////////////////////////////////////////////////////////////////////
// Procedure DrawScene is called whenever the scene needs to be drawn.
void Scene::DrawScene()
{
	// @TODO move this to beforeDraw...?
	int time_curFrame = glutGet(static_cast<gl::GLenum>(GLUT_ELAPSED_TIME));
	dt = static_cast<float>(time_curFrame - time_LastFrame) / 1000.0f;
	time_LastFrame = time_curFrame;

    // Calculate the light's position.
    float lPos[4] = {
       basePoint.x+lightDist*cos(lightSpin*rad)*sin(lightTilt*rad),
       basePoint.y+lightDist*sin(lightSpin*rad)*sin(lightTilt*rad),
       basePoint.z+lightDist*cos(lightTilt*rad),
       1.0 };

    // Set the viewport, and clear the screen
    glViewport(0,0,width, height);
    glClearColor(0.5,0.5, 0.5, 1.0);
    glClear(GL_COLOR_BUFFER_BIT| GL_DEPTH_BUFFER_BIT);

    // Compute Viewing and Perspective transformations.
    MAT4 WorldProj, WorldView, WorldInverse;

	WorldProj = Perspective(
		(static_cast<float>(width) * ry) / static_cast<float>(height),
		ry,
		frontClippingPlaneDist,
		backClippingPlanedist);
    
	if (movementType == MovementType::MT_ORBIT)
	{
		WorldView = Translate(cameraPan.x, cameraPan.y, -cameraZoom)*Rotate(0, cameraTilt - 90.0f)*Rotate(2, cameraSpin);
	}
	else if (movementType == MovementType::MT_GROUND)
	{
		WorldView =
			Rotate(0, cameraTilt - 90.0f)*
			Rotate(2, cameraSpin)*
			Translate(-cameraPos.x, -cameraPos.y, -cameraPos.z);
	}

    invert(&WorldView, &WorldInverse);

    // Use the lighting shader
    lightingProgram->Use();
    int programId = lightingProgram->programId;

    // Setup the perspective and viewing matrices for normal viewing.
    int loc;
    loc = glGetUniformLocation(programId, "WorldProj");
    glUniformMatrix4fv(loc, 1, GL_TRUE, WorldProj.Pntr());
    loc = glGetUniformLocation(programId, "WorldView");
    glUniformMatrix4fv(loc, 1, GL_TRUE, WorldView.Pntr());
    loc = glGetUniformLocation(programId, "WorldInverse");
    glUniformMatrix4fv(loc, 1, GL_TRUE, WorldInverse.Pntr());
    loc = glGetUniformLocation(programId, "lightPos");
    glUniform3fv(loc, 1, &(lPos[0]));  
    loc = glGetUniformLocation(programId, "mode");
    glUniform1i(loc, mode);  
    CHECKERROR;

    // Compute any continuously animating objects
    for (std::vector<Object*>::iterator m=animated.begin();  m<animated.end();  m++)
        (*m)->animTr = Rotate(2,atime);

    // Draw all objects
    objectRoot->Draw(lightingProgram, Identity);

    lightingProgram->Unuse();
    CHECKERROR;

}
