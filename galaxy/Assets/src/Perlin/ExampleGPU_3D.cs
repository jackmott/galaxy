using UnityEngine;
using System.Collections;

public class ExampleGPU_3D : MonoBehaviour 
{
	ImprovedPerlinNoise m_perlin;
	
	public int m_seed = 0;
	public float m_frequency = 1f;
	public float m_lacunarity = 1.0f;
	public float m_gain =  1f;
    public float m_octaves;
	
	void Start () 
	{
		m_perlin = new ImprovedPerlinNoise(m_seed);
		
		m_perlin.LoadResourcesFor3DNoise();
		
		GetComponent<Renderer>().material.SetTexture("_PermTable2D", m_perlin.GetPermutationTable2D());
		GetComponent<Renderer>().material.SetTexture("_Gradient3D", m_perlin.GetGradient3D());
	}
	
	void Update()
	{
		GetComponent<Renderer>().material.SetFloat("_Frequency", m_frequency);
		GetComponent<Renderer>().material.SetFloat("_Lacunarity", m_lacunarity);
		GetComponent<Renderer>().material.SetFloat("_Gain", m_gain);
        GetComponent<Renderer>().material.SetFloat("_Octaves", m_octaves);
    }
	
}
