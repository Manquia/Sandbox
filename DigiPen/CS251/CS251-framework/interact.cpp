
#include "framework.h"
#include <unordered_map>


extern Scene scene;       // Declared in framework.cpp, but used here.

int mouseX, mouseY;
bool shifted = false;
bool leftDown = false;
bool middleDown = false;
bool rightDown = false;

const float cameraPanSpeed = 0.05f;
const float cameraRotateSpeed = 10.2f;


const float cameraGroundSpeed = 5.5f;
const float cameraGroundMovementHeight = 2.0f;
const float cameraGroundTiltSpeed = 0.1f;
const float cameraGroundSpinSpeed = 0.22f;
const float cameraGroundTiltMaxUp = 90.0f;
const float cameraGroundTiltMaxDown = 80.0f;



// math stuffs
const float pi = 3.14159f;
const float toDegrees = (360.0f) / (2.0f*pi);
const float toRadians = (2.0f * pi) / 360.0f;

bool passiveMotionOn = false;


const float staticDT = 0.016f; // @TODO REMOVE @P2

void BeforeDraw();
void AfterDraw();

////////////////////////////////////////////////////////////////////////
// Called by GLUT when the scene needs to be redrawn. DO NOT USE THIS
// Unless its a whole new frame! it will mess with dt and Input in
// a way you may not expect
void ReDraw()
{
	BeforeDraw();
    scene.DrawScene();
    glutSwapBuffers();
	AfterDraw();
	// Assumed this is the begingin of a new frame, UpdateKeyStates
	Scene::UpdateKeyStates(scene.keyStates);
}


// NOT TESTED!!!!!!
glm::vec4 Mulitply(glm::vec4 vec, MAT4 mat)
{
	glm::vec4 out;
	out.x = vec.x * mat[0][0] + vec.y * mat[0][1] + vec.z * mat[0][2] + vec.w * mat[0][3];
	out.y = vec.x * mat[1][0] + vec.y * mat[1][1] + vec.z * mat[1][2] + vec.w * mat[1][3];
	out.z = vec.x * mat[2][0] + vec.y * mat[2][1] + vec.z * mat[2][2] + vec.w * mat[2][3];
	out.w = vec.x * mat[3][0] + vec.y * mat[3][1] + vec.z * mat[3][2] + vec.w * mat[3][3];
	return out;
}

void BeforeDraw()
{
	// Change movement type
	if (scene.GetKeyDigitalState('c') == DigitalState::Pressed)
	{
		if (scene.movementType == Scene::MovementType::MT_ORBIT)
		{
			scene.movementType = Scene::MovementType::MT_GROUND;
			// Setup scene stuff @TODO
		}
		else if (scene.movementType == Scene::MovementType::MT_GROUND)
		{
			scene.movementType = Scene::MovementType::MT_ORBIT;
			// Setup scene stuff @TODO
		}
	}

	if (scene.GetKeyDigitalState('p') == DigitalState::Pressed)
	{
		passiveMotionOn = !passiveMotionOn;
	}

	if (scene.movementType == Scene::MovementType::MT_GROUND)
	{
		const float step = cameraGroundSpeed * scene.dt;
		const float spin = scene.cameraSpin;
		if ((scene.GetKeyDigitalState('w') & DigitalState::Down) == DigitalState::Down) scene.cameraPos += step * glm::vec3(sin(spin * toRadians),  cos(spin * toRadians), 0.0f);
		if ((scene.GetKeyDigitalState('s') & DigitalState::Down) == DigitalState::Down) scene.cameraPos -= step * glm::vec3(sin(spin * toRadians),  cos(spin * toRadians), 0.0f);
		if ((scene.GetKeyDigitalState('d') & DigitalState::Down) == DigitalState::Down) scene.cameraPos += step * glm::vec3(cos(spin * toRadians), -sin(spin * toRadians), 0.0f);
		if ((scene.GetKeyDigitalState('a') & DigitalState::Down) == DigitalState::Down) scene.cameraPos -= step * glm::vec3(cos(spin * toRadians), -sin(spin * toRadians), 0.0f);

		// clamp camera to an offset off the gournd while in ground movement mode
		scene.cameraPos.z = scene.ground->HeightAt(scene.cameraPos.x, scene.cameraPos.y) + cameraGroundMovementHeight;
	}
}
void AfterDraw()
{
}


////////////////////////////////////////////////////////////////////////
// Function called to exit
void Quit(void *clientData)
{
    glutLeaveMainLoop();
}

////////////////////////////////////////////////////////////////////////
// Called by GLUT when the window size is changed.
void ReshapeWindow(int w, int h)
{
    if (w && h) {
        glViewport(0, 0, w, h); }
    scene.width = w;
    scene.height = h;

    // Force a redraw
    glutPostRedisplay();
}

////////////////////////////////////////////////////////////////////////
// Called by GLut for keyboard actions.
void KeyboardDown(unsigned char key, int x, int y)
{
    printf("key down %c(%d)\n", key, key);
    fflush(stdout);
  
	scene.KeyboardKeyDown(key);

    switch(key) {

    case 27: case 'q':       // Escape and 'q' keys quit the application
        exit(0);
    }
}

void KeyboardUp(unsigned char key, int x, int y)
{
    fflush(stdout);
	scene.KeyboardKeyUp(key);
}



////////////////////////////////////////////////////////////////////////
// Called by GLut when a mouse button changes state.
void MouseButton(int button, int state, int x, int y)
{        
    // Record the position of the mouse click.
    mouseX = x;
    mouseY = y;

    // Test if the SHIFT key was down for this mouse click
    shifted = glutGetModifiers() & GLUT_ACTIVE_SHIFT;

    // Ignore high order bits, set by some (stupid) GLUT implementation.
    button = button%8;

    // Figure out the mouse action, and handle accordingly
    if (button == 3 && shifted) { // Scroll light in
        scene.lightDist = pow(scene.lightDist, 1.0f/1.02f);
        printf("shifted scroll up\n"); }

    else if (button == 4 && shifted) { // Scroll light out
        scene.lightDist = pow(scene.lightDist, 1.02f);
        printf("shifted scroll down\n"); }

    else if (button == GLUT_LEFT_BUTTON) {
        leftDown = (state == GLUT_DOWN);
        printf("Left button down\n"); }

    else if (button == GLUT_MIDDLE_BUTTON) {
        middleDown = (state == GLUT_DOWN);
        printf("Middle button down\n");  }

    else if (button == GLUT_RIGHT_BUTTON) 
	{
        rightDown = (state == GLUT_DOWN);
        printf("Right button down\n");  
	}
    else if (button == 3 && scene.movementType == Scene::MovementType::MT_ORBIT) 
	{
        printf("scroll up\n"); 
		scene.cameraZoom *= 0.9f;
	}
    else if (button == 4 && scene.movementType == Scene::MovementType::MT_ORBIT)
	{
        printf("scroll down\n");
		scene.cameraZoom *= 1.1f;
	}

    // Force a redraw
    glutPostRedisplay();
    fflush(stdout);
}

////////////////////////////////////////////////////////////////////////
// Called by GLut when a mouse moves (while a button is down)
void MouseMotion(int x, int y)
{
    // Calculate the change in the mouse position
    int dx = x-mouseX;
    int dy = y-mouseY;

    if (leftDown && shifted)
	{  // Rotate light position
        scene.lightSpin += dx/3.0;
        scene.lightTilt -= dy/3.0; 
	}
    else if (leftDown) 
	{
		// Which movement type are we in?
		if (scene.movementType == Scene::MovementType::MT_ORBIT)// pan the camera
		{
			// Pan the camera in X,Y
			scene.cameraPan.x += dx * cameraPanSpeed * scene.cameraZoom * staticDT;
			scene.cameraPan.y += -dy * cameraPanSpeed * scene.cameraZoom * staticDT;
		}
    }

    if (middleDown && shifted) // move light
	{
		// Which movement type are we in?
		scene.lightDist = pow(scene.lightDist, 1.0f - dy / 200.0f);
	}
    else if (middleDown) 
	{
	}

    if (rightDown) 
	{
		// Which movement type are we in?
		if (scene.movementType == Scene::MovementType::MT_ORBIT)
		{
			// Rotate the camera Spin + Tilt
			scene.cameraSpin += dx * staticDT * cameraRotateSpeed;
			scene.cameraTilt += dy * staticDT * cameraRotateSpeed;
		}
    }


	if (scene.movementType == Scene::MovementType::MT_GROUND && !shifted)
	{
		// spin camera based on horizontal movement
		scene.cameraSpin += cameraGroundSpinSpeed * dx;

		// pitch camera based on vertical movmeent
		scene.cameraTilt = glm::clamp(scene.cameraTilt + cameraGroundTiltSpeed * dy, -cameraGroundTiltMaxDown, cameraGroundTiltMaxUp);
	}

	// Record this position
	mouseX = x;
	mouseY = y;

	// Force a redraw
	glutPostRedisplay();
}

void MouseMovedNoButton(int x, int y)
{
	// Calculate the change in the mouse position
	int dx = x - mouseX;
	int dy = y - mouseY;


	if (scene.movementType == Scene::MovementType::MT_GROUND && passiveMotionOn)
	{
		// spin camera based on horizontal movement
		scene.cameraSpin += cameraGroundSpinSpeed * dx;

		// pitch camera based on vertical movmeent
		scene.cameraTilt += cameraGroundTiltSpeed * dy;

		// @TODO for better movement in capture mode
		//glutWarpPointer(scene.width/2, scene.height/2);
	}


	// Record this position
	mouseX = x;
	mouseY = y;

	// Force a redraw
	glutPostRedisplay();
}

void InitInteraction()
{
    glutIgnoreKeyRepeat(true);
    
    glutDisplayFunc(&ReDraw);
    glutReshapeFunc(&ReshapeWindow);

    glutKeyboardFunc(&KeyboardDown);
    glutKeyboardUpFunc(&KeyboardUp);

    glutMouseFunc(&MouseButton);
    glutMotionFunc(&MouseMotion);
	glutPassiveMotionFunc(&MouseMovedNoButton);
}
