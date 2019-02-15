using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScreenWrapping {
	public class ScreenWrapper : MonoBehaviour {

		float left_side_of_screen, right_side_of_screen, top_of_screen, bottom_of_screen;
		void Start () {
			left_side_of_screen = -.5f + Camera.main.transform.position.x - Camera.main.orthographicSize * Screen.width / Screen.height;
			bottom_of_screen = -1.5f + Camera.main.transform.position.y - Camera.main.orthographicSize * Screen.height / Screen.width;

			right_side_of_screen = - left_side_of_screen;
			top_of_screen = - bottom_of_screen;
		}


		void Update () {
	        if(transform.position.x > right_side_of_screen)
	            transform.position = new Vector3(left_side_of_screen, transform.position.y, 0);
	        else if(transform.position.x < left_side_of_screen)
	            transform.position = new Vector3(right_side_of_screen, transform.position.y, 0);
	        else if(transform.position.y > top_of_screen)
	            transform.position = new Vector3(transform.position.x, bottom_of_screen, 0);
	        else if(transform.position.y < bottom_of_screen)
	            transform.position = new Vector3(transform.position.x, top_of_screen, 0);
	    }
	}
}