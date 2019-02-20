using UnityEngine;

public class RLS_LootSpot : MonoBehaviour
{
	[SerializeField] private BRS_ItemManager[] variants;

	public void SelectVariant()
	{
		var varChoice = variants[Random.Range(0, variants.Length)];

        Debug.Log(varChoice + " / " + variants.Length);

		//DESTROY all objects not selected!
        foreach(var variant in variants)
        {
            if(variant != varChoice)
            {
                Destroy(variant.gameObject);
            }
        }
	}
}
