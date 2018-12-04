using UnityEngine.UI;
using UnityEngine;

public enum degreeIncrement
{
    One = 1,
    Five = 5,
    Ten = 10
}

public class Compass : MonoBehaviour
{
	public RawImage CompassImage;
	public Transform Player;
	public Text CompassDirectionText;

    [Header("Readout Options")]
    [SerializeField] private bool ordinalLetters = true;//show N instead of 0 or S instead of 180
    [SerializeField] private degreeIncrement degreeInc = degreeIncrement.Five;// round to this number

	public void Update()
	{
		//Get a handle on the Image's uvRect
		CompassImage.uvRect = new Rect(Player.localEulerAngles.y / 360, 0, 1, 1);

		// Get a copy of your forward vector
		Vector3 forward = Player.transform.forward;

		// Zero out the y component of your forward vector to only get the direction in the X,Z plane
		forward.y = 0;

		//Clamp our angles to specified degree increments
		float headingAngle = Quaternion.LookRotation(forward).eulerAngles.y;//what is the player's angle
		headingAngle = (int)degreeInc * Mathf.RoundToInt(headingAngle / (int)degreeInc  );

        if (ordinalLetters)//convert the numbers to letters if pointing towards a direction (N/E/S/W)
        {
            ConvertAngleToLetter( (int)headingAngle );
        }
        else
        {
            CompassDirectionText.text = headingAngle.ToString();
        }
		
	}

    private void ConvertAngleToLetter(int angle)
    {//Set the text of Compass Degree Text to the clamped value, but change it to the letter if it is a True direction
        switch (angle)
        {
            case 0:
                //Do this
                CompassDirectionText.text = "N";
                break;
            case 360:
                //Do this
                CompassDirectionText.text = "N";
                break;
            case 45:
                //Do this
                CompassDirectionText.text = "NE";
                break;
            case 90:
                //Do this
                CompassDirectionText.text = "E";
                break;
            case 135:
                //Do this
                CompassDirectionText.text = "SE";
                break;
            case 180:
                //Do this
                CompassDirectionText.text = "S";
                break;
            case 225:
                //Do this
                CompassDirectionText.text = "SW";
                break;
            case 270:
                //Do this
                CompassDirectionText.text = "W";
                break;
            case 315:
                //Do this
                CompassDirectionText.text = "NW";
                break;
            default:
                CompassDirectionText.text = angle.ToString();
                break;
        }


    }
}