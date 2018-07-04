/////////////////////////////////////////////////////////////////////////
// Pixel shader for lighting
////////////////////////////////////////////////////////////////////////
#version 330

// These definitions agree with the ObjectIds enum in scene.h
const int     nullId	= 0;
const int     skyId	= 1;
const int     seaId	= 2;
const int     groundId	= 3;
const int     wallId	= 4;
const int     boxId	= 5;
const int     frameId	= 6;
const int     lPicId	= 7;
const int     rPicId	= 8;
const int     teapotId	= 9;
const int     spheresId	= 10;

#define M_PI 3.1415926535897932384626433832795

in vec3 normalVec, lightVec, worldPos, eyeVec;
in vec2 texCoord;

uniform int objectId;
uniform vec3 diffuse;	// Kd
uniform vec3 specular;	// Ks
uniform float shininess;// alpha exponent
uniform vec3 light;		// li
uniform vec3 ambient;	// la

float D(float HN, float alpha)
{
	return ((alpha + 2.0)/ (2.0 * M_PI)) * pow(HN, alpha);
}
vec3 F(vec3 Ks, float LH)
{
	return Ks + (1.0-Ks)* pow(1.0-LH, 5);
}

void main()
{
    vec3 N = normalize(normalVec);
    vec3 L = normalize(lightVec);
	vec3 V = normalize(eyeVec);
	vec3 H = normalize(L + V);
	float LN = max(dot(L,N), 0.0);
	float HN = max(dot(H,N), 0.0);
	float LH = max(dot(L,H), 0.0);

    vec3 Kd = diffuse;
	vec3 Ks = specular;
	float alpha = shininess;
    
    if (objectId==groundId || objectId==seaId) {
        ivec2 uv = ivec2(floor(200.0*texCoord));
        if ((uv[0]+uv[1])%2==0)
            Kd *= 0.9; }
    
	vec3 ambientOut = ambient * Kd;

	vec3 BRDFDiffuse =  Kd / M_PI;
	vec3 BRDFSpecular = (D(HN, alpha) * F(Ks, LH)) /
						(4 * LH*LH);

	vec3 diffSpecOut = (light * LN) * (BRDFDiffuse + BRDFSpecular);
	
	gl_FragColor.xyz = ambientOut + diffSpecOut;

	// Old Lighting
    //gl_FragColor.xyz = vec3(0.5,0.5,0.5)*Kd + Kd*max(dot(L,N),0.0);
}
