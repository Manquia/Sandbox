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

#define GLM_FORCE_RADIANS
#define GLM_SWIZZLE
#include <glm/glm.hpp>
using namespace glm;

#include "shader.h"
#include "shapes.h"
#include "scene.h"
#include "transform.h"

Object::Object(Shape* _shape, const int _objectId,
               const vec3 _diffuseColor, const vec3 _specularColor, const float _shininess)
    : diffuseColor(_diffuseColor), specularColor(_specularColor), shininess(_shininess),
      shape(_shape), objectId(_objectId)
     
{}


void Object::Draw(ShaderProgram* program, MAT4& objectTr)
{
    int loc = glGetUniformLocation(program->programId, "diffuse");
    glUniform3fv(loc, 1, &diffuseColor[0]);

    loc = glGetUniformLocation(program->programId, "specular");
    glUniform3fv(loc, 1, &specularColor[0]);

    loc = glGetUniformLocation(program->programId, "shininess");
    glUniform1f(loc, shininess);

    loc = glGetUniformLocation(program->programId, "objectId");
    glUniform1i(loc, objectId);

    MAT4 inv;
    invert(&objectTr, &inv);

    loc = glGetUniformLocation(program->programId, "ModelTr");
    glUniformMatrix4fv(loc, 1, GL_TRUE, objectTr.Pntr());
    
    loc = glGetUniformLocation(program->programId, "NormalTr");
    glUniformMatrix4fv(loc, 1, GL_TRUE, inv.Pntr());

	glm::vec3 light(3.0f, 3.0f, 3.0f);
	glm::vec3 ambient(0.4f, 0.4f, 0.4f);

	loc = glGetUniformLocation(program->programId, "light");
	glUniform3fv(loc, 1, &(light[0]));

	loc = glGetUniformLocation(program->programId, "ambient");
	glUniform3fv(loc, 1, &(ambient[0]));

    // If this oject has an associated texture, this is the place to
    // load the texture unto a texture-unit of your choice and inform
    // the shader program of the texture-unit number:
	//unsigned int unitId = 0;
	if(textures.size() > 0)
	{
		auto &tex = textures[0];
		glActiveTexture(GL_TEXTURE0);					// Choose unit 0
		glBindTexture(GL_TEXTURE_2D, tex->textureId);			// Load the texture
		loc=glGetUniformLocation(program->programId, "texDif");	// Find the sampler2D
		glUniform1i(loc, 0);						// Tell the shader program
	}

     //  glActiveTexture(GL_TEXTURE0);					// Choose unit 0
     //  glBindTexture(GL_TEXTURE_2D, textureId);			// Load the texture
     //  loc=glGetUniformLocation(program->programId, "samplerName");	// Find the sampler2D
     //  glUniform1i(loc, 0);						// Tell the shader program
    
    // Draw this object's triangle
    if (shape) shape->DrawVAO();

	for(auto& tex : this->textures)
	{
		tex->Unbind();
	}

    // Recursively draw each sub-objects, each with its own transformation.
    for (int i=0;  i<instances.size();  i++) {
        MAT4 itr = objectTr*instances[i].second*animTr;
        instances[i].first->Draw(program, itr); }
}
