
#include "framework.h"

extern Scene scene;       // Declared in framework.cpp, but used here.

// Some globals used for mouse handling.
// input state
enum DigitalState
{
	Up = 0,
	Down = 0,
	Pressed = 0,
	Released = 0,
};
DigitalState mouseButtonStates[16] = {};
int mouseX, mouseY;
bool shifted = false;
bool leftDown = false;
bool middleDown = false;
bool rightDown = false;

const float cameraPanSpeed = 0.05f;
const float cameraRotateSpeed = 10.2f;

const float staticDT = 0.016f; // @TODO REMOVE

////////////////////////////////////////////////////////////////////////
// Called by GLUT when the scene needs to be redrawn.
void ReDraw()
{
    scene.DrawScene();
    glutSwapBuffers();
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
  
    switch(key) {
    case 27: case 'q':       // Escape and 'q' keys quit the application
        exit(0);
    }
}

void KeyboardUp(unsigned char key, int x, int y)
{
    fflush(stdout);
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
    else if (button == 3) 
	{
        printf("scroll up\n"); 
		scene.cameraZoom *= 0.9f;
	}
    else if (button == 4) 
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

    if (leftDown && shifted) {  // Rotate light position
        scene.lightSpin += dx/3.0;
        scene.lightTilt -= dy/3.0; }

    else if (leftDown) {
    }

    if (middleDown && shifted) // move light
	{
		scene.lightDist = pow(scene.lightDist, 1.0f - dy / 200.0f);
		if (scene.movementType == Scene::MovementType::MT_ORBIT)
		{
		}
		else if (scene.movementType == Scene::MovementType::MT_GROUND)
		{
		}
	}
    else if (middleDown) 
	{
		if (scene.movementType == Scene::MovementType::MT_ORBIT)// pan the camera
		{
			scene.cameraPan.x += dx  * cameraPanSpeed * scene.cameraZoom * staticDT;
			scene.cameraPan.y += -dy * cameraPanSpeed * scene.cameraZoom * staticDT;
		}
		else if (scene.movementType == Scene::MovementType::MT_GROUND)
		{
		}
	}

    if (rightDown) 
	{
		if (scene.movementType == Scene::MovementType::MT_ORBIT)
		{
			scene.cameraSpin += dx * staticDT * cameraRotateSpeed;
			scene.cameraTilt += dy * staticDT * cameraRotateSpeed;
		}
		else if (scene.movementType == Scene::MovementType::MT_GROUND)
		{
		}
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
}
