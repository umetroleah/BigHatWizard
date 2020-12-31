using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : MonoBehaviour
{
    public Material mat;

    public float maxDisturbanceX = 1f;
    public float maxDisturbanceY = 1f;
    public float randomMovementX = 0.1f;
    public float randomMovementY = 0.1f;
    public float dampingX = 0.1f;
    public float dampingY = 0.1f;

    [SerializeField]
    public Vector3[] vertices;
    private Vector3[] vertexVelocities;

    private Mesh mesh;
    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();

        mesh.vertices = vertices;
        vertexVelocities = new Vector3[vertices.Length];

        mesh.triangles = new int[] { 0, 1, 5, 1, 2, 4, 2, 3, 4, 4, 5, 1, 5, 6, 0, 6, 7, 11, 7, 8, 10, 8, 9, 10, 10, 11, 7, 11, 0, 6 };

        GetComponent<MeshRenderer>().material = mat;

        GetComponent<MeshFilter>().mesh = mesh;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;

        Vector3[] newVertices = new Vector3[vertices.Length];

        for (int i = 0; i<vertices.Length; i++)
        {
            //Keep velocites from before
            float moveX = vertexVelocities[i].x;
            float moveY = vertexVelocities[i].y;

            //Move back towards center, but have random movement
            moveX += 2f * Sigmoid(vertices[i].x - mesh.vertices[i].x) - 1f 
                - Random.value * randomMovementX;
            moveY += 2f * Sigmoid(vertices[i].y - mesh.vertices[i].y) - 1f 
                - Random.value * randomMovementY;

            moveX *= 1 - dampingX;
            moveY *= 1 - dampingY;


            //Do not move beyond certain point
            if (mesh.vertices[i].x + moveX >= vertices[i].x + maxDisturbanceX)
                moveX = 0f;
            if (mesh.vertices[i].x + moveX <= vertices[i].x - maxDisturbanceX)
                moveX = 0f;
            if (mesh.vertices[i].y + moveY >= vertices[i].y + maxDisturbanceY)
                moveY = 0f;
            if (mesh.vertices[i].y + moveY <= vertices[i].y - maxDisturbanceY)
                moveY = 0f;

            if(velocity.x > 0f)
            {
                print(velocity.x);
            }
            moveX -= Mathf.Clamp(velocity.x / 30, -maxDisturbanceX / 2, maxDisturbanceX / 2);
            moveY -= Mathf.Clamp(velocity.y / 30, -maxDisturbanceY / 2, maxDisturbanceY / 2);

            moveX *= vertices[i].z;
            moveY *= vertices[i].z;


            //Set velocties to be kept
            vertexVelocities[i].x = moveX;
            vertexVelocities[i].y = moveY;

            //print(mesh.vertices[i] + "   " + moveX);

            //Move points
            newVertices[i] = new Vector3(mesh.vertices[i].x + moveX, mesh.vertices[i].y + moveY, 0f);

            //print(newVertices[i]);
        }

        mesh.vertices = newVertices;
    }


    public static float Sigmoid(float value)
    {
        return 1.0f / (1.0f + (float)Mathf.Exp(-value));
    }
}
