using UnityEngine;
using System.Collections;

public class DWGDestroyer : MonoBehaviour 
{

	public float radius = 2;
	public float force = 50f;
	public LayerMask destructibleLayer;
	
	void OnCollisionEnter(Collision col)
	{
			ExplodeForce();
			Destroy(GetComponent<DWGDestroyer>());
	}
	
	// Explode force by radius only if a destructible tag is found
	void ExplodeForce()
	{	
		Vector3 explodePos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explodePos, radius, destructibleLayer); 
		

		foreach (Collider hit in colliders){
			if(hit.gameObject.layer == destructibleLayer)
			{
				Rigidbody rigid = hit.GetComponent<Rigidbody>();
				if(rigid != null)
				{
					rigid.isKinematic = false;
					rigid.AddExplosionForce(force, explodePos, radius);
				}
			}
		}
	}
}
