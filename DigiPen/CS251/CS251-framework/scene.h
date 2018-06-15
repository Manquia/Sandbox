////////////////////////////////////////////////////////////////////////
// The scene class contains all the parameters needed to define and
// draw the (really) simple scene, including:
//   * Geometry (in a display list)
//   * Light parameters
//   * Material properties
//   * Viewport size parameters
//   * Viewing transformation values
//   * others ...
//
// Some of these parameters are set when the scene is built, and
// others are set by the framework in response to user mouse/keyboard
// interactions.  All of them should be used to draw the scene.

#include "shapes.h"
#include "object.h"
#include "texture.h"
#include <unordered_map>


// @TODO Move this elsewhere where it will be more accessable
// Some globals used for mouse handling.
// input state
enum DigitalState
{
	Pressed = 3, //0b11,
	Released = 2, //0b10,
	Up = 0, //0b00,
	Down = 1, //0b01,
};

enum ObjectIds {
    nullId	= 0,
    skyId	= 1,
    seaId	= 2,
    groundId	= 3,
    wallId	= 4,
    boxId	= 5,
    frameId	= 6,
    lPicId	= 7,
    rPicId	= 8,
    teapotId	= 9,
    spheresId	= 10,
};

class Shader;

class Scene
{
public:
    // Viewing transformation parameters (suggested) FIXME: This is a
    // good place for the transformation values which are set by the
    // user mouse/keyboard actions and used in DrawScene to create the
    // transformation matrices.

	//Scene Veriables
	float ry;
	float frontClippingPlaneDist;
	float backClippingPlanedist;
	float cameraTilt;
	float cameraSpin;
	float cameraZoom;
	int elapsedTime;
	glm::vec2 cameraPan;
	enum MovementType
	{
		MT_ORBIT,
		MT_GROUND,
	};
	MovementType movementType;


    ProceduralGround* ground;

    // Light position parameters
    float lightSpin, lightTilt, lightDist;

    vec3 basePoint;  // Records where the scene building is centered.
    int mode; // Extra mode indicator hooked up to number keys and sent to shader
    
    // Viewport
    int width, height;

	// Time
	int time_startProgram;
	int time_LastFrame;
	float dt;

    // All objects in the scene are children of this single root object.
    Object* objectRoot;
    std::vector<Object*> animated;

    // Shader programs
    ShaderProgram* lightingProgram;

    //void append(Object* m) { objects.push_back(m); }

    void InitializeScene();
    void DrawScene();



	std::unordered_map<unsigned char, DigitalState> keyStates = {};

	DigitalState GetKeyDigitalState(unsigned char key)
	{
		if (keyStates.find(key) == keyStates.end())
			return DigitalState::Up;
		else
			return keyStates[key];
	}

	static void UpdateKeyStates(std::unordered_map<unsigned char, DigitalState>& keystates)
	{
		for (auto &key : keystates)
		{
			if (key.second == DigitalState::Pressed)
			{
				key.second = DigitalState::Down;
			}
			if (key.second == DigitalState::Released)
			{
				key.second = DigitalState::Up;
			}
		}
	}

	void KeyboardKeyDown(unsigned char key)
	{
		// add key state if we previously hadn't used it
		if (keyStates.find(key) == keyStates.end())
		{
			// Assumed that we were in up state since there is no record
			keyStates[key] = DigitalState::Pressed;
		}
		else
		{
			DigitalState oldState = keyStates[key];

			// ignore repreat key events to press down
			if ((oldState & DigitalState::Down) != DigitalState::Down)
			{
				keyStates[key] = DigitalState::Pressed;
			}
		}
	}
	void KeyboardKeyUp(unsigned char key)
	{
		// add key state if we previously hadn't used it
		// Assumed that we were in up state
		keyStates[key] = DigitalState::Released;
	}

};
