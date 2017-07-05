using UnityEngine;
using System;
using System.Collections;

class PiranhaFlock: MonoBehaviour
{
    public Transform baricentro;
    public int layer;
	float tooClose;
	Vector3 dir;

	void Start()
    {
		StartCoroutine(flock());
		dir = transform.forward;
	}

	void Update()
    {
		//Move toward calculated direction
        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, dir, FlockGlobals.instance.m_rotSpeed * Time.deltaTime, 0.0f));
        transform.Translate(transform.forward * FlockGlobals.instance.m_speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
	}

	public IEnumerator flock()
    {
		while (true)
        {
			Vector3 tmp = Vector3.zero, tmpdir = Vector3.zero;

            Collider[] neighbours = Physics.OverlapSphere(transform.position, FlockGlobals.instance.m_sight_radius, layer);
            tooClose = FlockGlobals.instance.m_sight_radius ;

			tmp += alignment(neighbours);
			yield return null;

			tmp += cohesion(neighbours);
			yield return null;

			tmp += separation(neighbours);

			tmp.Normalize();

			tmpdir = baricentro.position - transform.position;
			tmpdir.Normalize();

			//  The nearer the closer boid, the greater the flock component factor.
            dir = Mathf.Clamp((1f - tooClose/FlockGlobals.instance.m_sight_radius), 0.0f, 0.6f) * tmp + Mathf.Clamp((tooClose/FlockGlobals.instance.m_sight_radius), 0.4f, 0.8f) * tmpdir;
			/*
             * The two component are clamped because, otherwise, if the flocklings 
			 *  are too close to one another they will ignore the target
             */         
			dir.Normalize();
			yield return null;
		}
	}

	Vector3 alignment(Collider[] neighbours)
    {
		Vector3 alignment = Vector3.zero;
		foreach (Collider c in neighbours)
        {
			if(isVisible(c.gameObject.transform.position))
			    alignment += c.gameObject.transform.forward;	
		}
        return alignment.normalized;
	}

	Vector3 cohesion(Collider[] neighbours)
    {
		Vector3 cohesion = Vector3.zero;
		float counter = 0f;
		foreach(Collider c in neighbours)
        {
            if (isVisible(c.gameObject.transform.position))
            {
                counter++;
                cohesion += c.transform.position;
            }
		}

		cohesion -= transform.position;
		cohesion /= counter;
		cohesion = cohesion - transform.position;
        return cohesion.normalized;
	}

	Vector3 separation(Collider[] neighbours)
    {
		Vector3 separation = Vector3.zero;
		Vector3 tmp;

		foreach (Collider c in neighbours)
        {
			if( c.transform.position == transform.position || !isVisible (c.gameObject.transform.position))
			   	continue;
            
			tmp = (transform.position - c.gameObject.transform.position);
			tooClose = tmp.magnitude < tooClose ? tmp.magnitude : tooClose;
			
			separation += tmp.normalized / Mathf.Clamp(tmp.magnitude, 0.0001f, float.PositiveInfinity);
		}

		return separation.normalized;
	}

	bool isVisible(Vector3 pos)
    {
        return Vector3.Dot(transform.forward, (pos - transform.position).normalized) > FlockGlobals.instance.m_visionAngle;
	}
}