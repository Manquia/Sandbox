#include <fstream>

#include "math.h"
#include "transform.h"

// This is used to communicate a MAT4's address to OpenGL
float* MAT4::Pntr()
{
    return &(M[0][0]);
}

// Return a rotation matrix around an axis (0:X, 1:Y, 2:Z) 
// by an angle measured in degrees.
// NOTE:  Make sure to convert degrees to radians before using sin and cos:
//        radians = degrees*PI/180
const float pi = 3.14159f;
const float toDegrees = (360.0f) / (2.0f*pi);
const float toRadians = (2.0f * pi) / 360.0f;

// Theta is in degrees!!!
MAT4 Rotate(const int i, const float theta)
{
	MAT4 R = {};
	const float thetaRad = theta * toRadians;
	const int j = (i + 1) % 3;
	const int k = (j + 1) % 3;
	const float cosTheta = cosf(thetaRad);
	const float sinTheta = sinf(thetaRad);

	R.M[j][j] = R.M[k][k] = cosTheta;
	R.M[j][k] = -sinTheta;
	R.M[k][j] = sinTheta;

    return R;
}

// Return a scale matrix
MAT4 Scale(const float x, const float y, const float z)
{
	MAT4 S = {};
	S.M[0][0] = x;
	S.M[1][1] = y;
	S.M[2][2] = z;

    return S;
}

// Return a translation matrix
MAT4 Translate(const float x, const float y, const float z)
{
	MAT4 T = {};
	T.M[0][3] = x;
	T.M[1][3] = y;
	T.M[2][3] = z;
    return T;
}

// Returns a perspective projection matrix
MAT4 Perspective(const float rx, const float ry,
             const float front, const float back)
{
	MAT4 P = {};
	P.M[0][0] = 1.0f / rx;
	P.M[1][1] = 1.0f / ry;
	P.M[2][2] = -(back + front) / (back - front);
	P.M[2][3] = (-2.0f * front * back) / (back - front);
	P.M[3][2] = -1;
	P.M[3][3] = 0.0f;

    return P;
}

#include <xmmintrin.h>
//#define USE_SSE

// Multiplies two 4x4 matrices
MAT4 operator* (const MAT4 a, const MAT4 b)
{  
	//@ Speed M is initialized to identity
	MAT4 M = {};
#ifdef USE_SSE
	__m128 otherRow0 = _mm_loadu_ps(b.M[0]);
	__m128 otherRow1 = _mm_loadu_ps(b.M[1]);
	__m128 otherRow2 = _mm_loadu_ps(b.M[2]);
	__m128 otherRow3 = _mm_loadu_ps(b.M[3]);

	__m128 newRow0 = _mm_mul_ps(otherRow0, _mm_set1_ps(a.M[0][0]));
	newRow0 = _mm_add_ps(newRow0, _mm_mul_ps(otherRow1, _mm_set1_ps(a.M[0][1])));
	newRow0 = _mm_add_ps(newRow0, _mm_mul_ps(otherRow2, _mm_set1_ps(a.M[0][2])));
	newRow0 = _mm_add_ps(newRow0, _mm_mul_ps(otherRow3, _mm_set1_ps(a.M[0][3])));

	__m128 newRow1 = _mm_mul_ps(otherRow0, _mm_set1_ps(a.M[1][0]));
	newRow1 = _mm_add_ps(newRow1, _mm_mul_ps(otherRow1, _mm_set1_ps(a.M[1][1])));
	newRow1 = _mm_add_ps(newRow1, _mm_mul_ps(otherRow2, _mm_set1_ps(a.M[1][2])));
	newRow1 = _mm_add_ps(newRow1, _mm_mul_ps(otherRow3, _mm_set1_ps(a.M[1][3])));

	__m128 newRow2 = _mm_mul_ps(otherRow0, _mm_set1_ps(a.M[2][0]));
	newRow2 = _mm_add_ps(newRow2, _mm_mul_ps(otherRow1, _mm_set1_ps(a.M[2][1])));
	newRow2 = _mm_add_ps(newRow2, _mm_mul_ps(otherRow2, _mm_set1_ps(a.M[2][2])));
	newRow2 = _mm_add_ps(newRow2, _mm_mul_ps(otherRow3, _mm_set1_ps(a.M[2][3])));

	__m128 newRow3 = _mm_mul_ps(otherRow0, _mm_set1_ps(a.M[3][0]));
	newRow3 = _mm_add_ps(newRow3, _mm_mul_ps(otherRow1, _mm_set1_ps(a.M[3][1])));
	newRow3 = _mm_add_ps(newRow3, _mm_mul_ps(otherRow2, _mm_set1_ps(a.M[3][2])));
	newRow3 = _mm_add_ps(newRow3, _mm_mul_ps(otherRow3, _mm_set1_ps(a.M[3][3])));

	_mm_storeu_ps(M.M[0], newRow0);
	_mm_storeu_ps(M.M[1], newRow1);
	_mm_storeu_ps(M.M[2], newRow2);
	_mm_storeu_ps(M.M[3], newRow3);
#else

	for (int x = 0; x < 4; x++)
	{
		for (int y = 0; y < 4; y++)
		{
			M.M[x][y] = 0.0f; // set value to 0 for accumulations of multiplications
			for (int z = 0; z < 4; z++)
			{
				M.M[x][y] += a.M[x][z] * b.M[z][y];
			}
		}
	}


#endif
#undef USE_SSE
    return M;
}


////////////////////////////////////////////////////////////////////////
// Calculates the inverse of a matrix by performing the gaussian
// matrix reduction with partial pivoting followed by
// back/substitution with the loops manually unrolled.
//
// Taken from Mesa implementation of OpenGL:  http://mesa3d.sourceforge.net/
////////////////////////////////////////////////////////////////////////
#define MAT(m,r,c) ((*m)[r][c])
#define SWAP_ROWS(a, b) { double *_tmp = a; (a)=(b); (b)=_tmp; }

int invert(const MAT4* mat, MAT4* inv)
{
   double wtmp[4][8];
   double m0, m1, m2, m3, s;
   double *r0, *r1, *r2, *r3;

   r0 = wtmp[0], r1 = wtmp[1], r2 = wtmp[2], r3 = wtmp[3];

   r0[0] = MAT(mat,0,0);
   r0[1] = MAT(mat,0,1);
   r0[2] = MAT(mat,0,2);
   r0[3] = MAT(mat,0,3);
   r0[4] = 1.0;
   r0[5] = r0[6] = r0[7] = 0.0;
   
   r1[0] = MAT(mat,1,0);
   r1[1] = MAT(mat,1,1);
   r1[2] = MAT(mat,1,2);
   r1[3] = MAT(mat,1,3);
   r1[5] = 1.0, r1[4] = r1[6] = r1[7] = 0.0;

   r2[0] = MAT(mat,2,0);
   r2[1] = MAT(mat,2,1);
   r2[2] = MAT(mat,2,2);
   r2[3] = MAT(mat,2,3);
   r2[6] = 1.0, r2[4] = r2[5] = r2[7] = 0.0;

   r3[0] = MAT(mat,3,0);
   r3[1] = MAT(mat,3,1);
   r3[2] = MAT(mat,3,2);
   r3[3] = MAT(mat,3,3);
   r3[7] = 1.0, r3[4] = r3[5] = r3[6] = 0.0;

   /* choose pivot - or die */
   if (fabs(r3[0])>fabs(r2[0])) SWAP_ROWS(r3, r2);
   if (fabs(r2[0])>fabs(r1[0])) SWAP_ROWS(r2, r1);
   if (fabs(r1[0])>fabs(r0[0])) SWAP_ROWS(r1, r0);
   if (0.0 == r0[0])  return 0;

   /* eliminate first variable     */
   m1 = r1[0]/r0[0]; m2 = r2[0]/r0[0]; m3 = r3[0]/r0[0];
   s = r0[1]; r1[1] -= m1 * s; r2[1] -= m2 * s; r3[1] -= m3 * s;
   s = r0[2]; r1[2] -= m1 * s; r2[2] -= m2 * s; r3[2] -= m3 * s;
   s = r0[3]; r1[3] -= m1 * s; r2[3] -= m2 * s; r3[3] -= m3 * s;
   s = r0[4];
   if (s != 0.0) { r1[4] -= m1 * s; r2[4] -= m2 * s; r3[4] -= m3 * s; }
   s = r0[5];
   if (s != 0.0) { r1[5] -= m1 * s; r2[5] -= m2 * s; r3[5] -= m3 * s; }
   s = r0[6];
   if (s != 0.0) { r1[6] -= m1 * s; r2[6] -= m2 * s; r3[6] -= m3 * s; }
   s = r0[7];
   if (s != 0.0) { r1[7] -= m1 * s; r2[7] -= m2 * s; r3[7] -= m3 * s; }

   /* choose pivot - or die */
   if (fabs(r3[1])>fabs(r2[1])) SWAP_ROWS(r3, r2);
   if (fabs(r2[1])>fabs(r1[1])) SWAP_ROWS(r2, r1);
   if (0.0 == r1[1])  return 0;

   /* eliminate second variable */
   m2 = r2[1]/r1[1]; m3 = r3[1]/r1[1];
   r2[2] -= m2 * r1[2]; r3[2] -= m3 * r1[2];
   r2[3] -= m2 * r1[3]; r3[3] -= m3 * r1[3];
   s = r1[4]; if (0.0 != s) { r2[4] -= m2 * s; r3[4] -= m3 * s; }
   s = r1[5]; if (0.0 != s) { r2[5] -= m2 * s; r3[5] -= m3 * s; }
   s = r1[6]; if (0.0 != s) { r2[6] -= m2 * s; r3[6] -= m3 * s; }
   s = r1[7]; if (0.0 != s) { r2[7] -= m2 * s; r3[7] -= m3 * s; }

   /* choose pivot - or die */
   if (fabs(r3[2])>fabs(r2[2])) SWAP_ROWS(r3, r2);
   if (0.0 == r2[2])  return 0;

   /* eliminate third variable */
   m3 = r3[2]/r2[2];
   r3[3] -= m3 * r2[3];
   r3[4] -= m3 * r2[4];
   r3[5] -= m3 * r2[5];
   r3[6] -= m3 * r2[6];
   r3[7] -= m3 * r2[7];

   /* last check */
   if (0.0 == r3[3]) return 0;

   s = 1.0F/r3[3];             /* now back substitute row 3 */
   r3[4] *= s; r3[5] *= s; r3[6] *= s; r3[7] *= s;

   m2 = r2[3];                 /* now back substitute row 2 */
   s  = 1.0F/r2[2];
   r2[4] = s * (r2[4] - r3[4] * m2);
   r2[5] = s * (r2[5] - r3[5] * m2);
   r2[6] = s * (r2[6] - r3[6] * m2);
   r2[7] = s * (r2[7] - r3[7] * m2);
   m1 = r1[3];
   r1[4] -= r3[4] * m1;
   r1[5] -= r3[5] * m1;
   r1[6] -= r3[6] * m1;
   r1[7] -= r3[7] * m1;
   m0 = r0[3];
   r0[4] -= r3[4] * m0;
   r0[5] -= r3[5] * m0;
   r0[6] -= r3[6] * m0;
   r0[7] -= r3[7] * m0;

   m1 = r1[2];                 /* now back substitute row 1 */
   s  = 1.0F/r1[1];
   r1[4] = s * (r1[4] - r2[4] * m1);
   r1[5] = s * (r1[5] - r2[5] * m1);
   r1[6] = s * (r1[6] - r2[6] * m1);
   r1[7] = s * (r1[7] - r2[7] * m1);
   m0 = r0[2];
   r0[4] -= r2[4] * m0;
   r0[5] -= r2[5] * m0;
   r0[6] -= r2[6] * m0;
   r0[7] -= r2[7] * m0;

   m0 = r0[1];                 /* now back substitute row 0 */
   s  = 1.0F/r0[0];
   r0[4] = s * (r0[4] - r1[4] * m0);
   r0[5] = s * (r0[5] - r1[5] * m0);
   r0[6] = s * (r0[6] - r1[6] * m0);
   r0[7] = s * (r0[7] - r1[7] * m0);

   MAT(inv,0,0) = r0[4];
   MAT(inv,0,1) = r0[5],
   MAT(inv,0,2) = r0[6];
   MAT(inv,0,3) = r0[7],
   MAT(inv,1,0) = r1[4];
   MAT(inv,1,1) = r1[5],
   MAT(inv,1,2) = r1[6];
   MAT(inv,1,3) = r1[7],
   MAT(inv,2,0) = r2[4];
   MAT(inv,2,1) = r2[5],
   MAT(inv,2,2) = r2[6];
   MAT(inv,2,3) = r2[7],
   MAT(inv,3,0) = r3[4];
   MAT(inv,3,1) = r3[5],
   MAT(inv,3,2) = r3[6];
   MAT(inv,3,3) = r3[7];

   return 1;
}
