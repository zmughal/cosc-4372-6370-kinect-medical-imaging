/* skybox.cs		Skybox Script
 * author:			SoulofDeity
 * 
 * YouTube: http://youtu.be/dHoNJjtZ7yc
 * Download: http://www.mediafire.com/download/1f22csav1ctus17/skybox.7z
 * 
 * Features:
 *	- animation via rotation on y axis, speed adjustable
 *  - skybox tinting (requires skybox shader)
 * 	- smooth fading color transitioning over a specified
 *    period of time
 *  - multiple skyboxes
 *  - smooth fading skybox transitioning over a specified
 *    period of time (works in conjunction with color
 *    fading as well)
 * 
 * Technical Details:
 *	- uses user layer 8 for the skybox layer, which is
 *    rendered at a depth of -1
 *  - skybox texture arrays are stored in the order:
 *       front, back, left, right, top, bottom
 *       z+, z-, x-, x+, y+, y-
 ***************************************************************/
using UnityEngine;
using System.Collections;

public class skybox : MonoBehaviour
{
	enum SkyboxType
    {
		DAY			= 0,
		NIGHT		= 1
	};

	static Transform globalSkybox;
	static bool isSkyboxColorTransitioning = false;
	static bool isSkyboxTextureTransitioning = false;

	public float rotationSpeed;
	public Shader shader;
	public Color hue = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Texture[] daySkybox = new Texture[6];
	
	private Mesh cubeMesh;
	
	void Start()
    {
        initCubeMesh();
		
        globalSkybox = createSkybox("skybox", daySkybox);
        globalSkybox.localScale = Vector3.one * 100;
        globalSkybox.gameObject.layer = 0;
	}
	
	void Update()
    {
		globalSkybox.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0));
	}
	
	Transform createSkybox(string name, Texture[] textures)
    {
		Transform sb = (new GameObject(name)).transform;
		sb.gameObject.layer = 8;
		MeshFilter mf = (MeshFilter)sb.gameObject.AddComponent("MeshFilter");
		mf.mesh = cubeMesh;
		MeshRenderer mr = (MeshRenderer)sb.gameObject.AddComponent("MeshRenderer");
		mr.enabled = true;
		mr.castShadows = false;
		mr.receiveShadows = false;
		mr.materials = new Material[6];
		for (int i = 0; i < 6; i++) {
			mr.materials[i] = new Material(shader);
			mr.materials[i].shader = shader;
			mr.materials[i].color = hue;
			mr.materials[i].mainTexture = textures[i];
		}
		return sb;
	}
	
	void initCubeMesh()
    {
		cubeMesh = new Mesh();
        cubeMesh.vertices = new Vector3[]
        {
            new Vector3(-1,-1, 1),			// front
            new Vector3(-1, 1, 1),
            new Vector3( 1, 1, 1),
            new Vector3( 1,-1, 1),
			
            new Vector3(-1,-1,-1),			// back
            new Vector3(-1, 1,-1),
            new Vector3( 1, 1,-1),
            new Vector3( 1,-1,-1),
			
            new Vector3(-1,-1,-1),			// left
            new Vector3(-1, 1,-1),
            new Vector3(-1, 1, 1),
            new Vector3(-1,-1, 1),
			
            new Vector3( 1,-1,-1),			// right
            new Vector3( 1, 1,-1),
            new Vector3( 1, 1, 1),
            new Vector3( 1,-1, 1),
			
            new Vector3(-1, 1,-1),			// top
            new Vector3(-1, 1, 1),
            new Vector3( 1, 1, 1),
            new Vector3( 1, 1,-1),
			
            new Vector3(-1,-1,-1),			// bottom
            new Vector3(-1,-1, 1),
            new Vector3( 1,-1, 1),
            new Vector3( 1,-1,-1)
        };
        cubeMesh.uv = new Vector2[]
        {
            new Vector2(0, 0),				// front
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),

            new Vector2(1, 0),				// back
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
			
            new Vector2(0, 0),				// left
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
			
            new Vector2(1, 0),				// right
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
			
            new Vector2(1, 0),				// top
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(0, 0),
			
            new Vector2(1, 1),				// bottom
            new Vector2(1, 0),
            new Vector2(0, 0),
            new Vector2(0, 1)
        };
		cubeMesh.subMeshCount = 6;
        cubeMesh.SetTriangles(new int[] { 0, 1, 2, 2, 3, 0 }, 0);	// front
        cubeMesh.SetTriangles(new int[] { 6, 5, 4, 4, 7, 6 }, 1);	// back
        cubeMesh.SetTriangles(new int[] { 8, 9, 10, 10, 11, 8 }, 2);	// left
        cubeMesh.SetTriangles(new int[] { 14, 13, 12, 12, 15, 14 }, 3);	// right
        cubeMesh.SetTriangles(new int[] { 18, 17, 16, 16, 19, 18 }, 4);	// top
        cubeMesh.SetTriangles(new int[] { 20, 21, 22, 22, 23, 20 }, 5);	// bottom
		cubeMesh.RecalculateNormals();
        cubeMesh.Optimize();
	}
}
