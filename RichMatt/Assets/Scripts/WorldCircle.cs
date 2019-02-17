using UnityEngine;

class WorldCircle
{
    // This one does all the work
    public static void ConfigureWorldCircle(LineRenderer renderer, float radius, float height, int segments = 64, bool renderInWorldSpace = false)

    {
		float x = 0;
		float y = height;
		float z = 0;
		float angle = 0;

        renderer.positionCount = segments;
        renderer.useWorldSpace = false;

		for (int i = 0; i < segments; i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * angle) * radius;
			z = Mathf.Cos (Mathf.Deg2Rad * angle) * radius;

			renderer.SetPosition (i, new Vector3(x,y,z) );

			angle += (360f / segments);
		}
	}
}