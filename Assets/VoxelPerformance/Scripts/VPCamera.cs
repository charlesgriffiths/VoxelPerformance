// This script is included with the VoxelPerformance asset to assist with moving around a voxel terrain for debugging.

using UnityEngine;


public class VPCamera : MonoBehaviour
{
public bool locked = true;
public float speed = 100f;

float angleX;
float angleY;


  void Update()
  {
    mouse();
    move();
  }


  void mouse()
  {
  bool cursorLock = Input.GetKey( KeyCode.Tab ) ? false : locked;

    Cursor.lockState = cursorLock ? CursorLockMode.Locked : CursorLockMode.None;
    Cursor.visible = !cursorLock;

    angleX += Input.GetAxis( "Mouse X" );
    angleY += Input.GetAxis( "Mouse Y" );

    transform.eulerAngles = new Vector3( -angleY, angleX, 0 );
  }


  void move()
  {
  Vector3 velocity = new Vector3( Input.GetAxis( "Horizontal" ) * speed, 0, Input.GetAxis( "Vertical" ) * speed );

    transform.Translate( velocity * Time.deltaTime );
  }
}

