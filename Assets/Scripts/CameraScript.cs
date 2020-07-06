using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
	private void Start()
	{
		this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5f * 0.85f;
	}
	public IEnumerator ZoomIn()
	{
		for (int i = 1; i <= 15; i++)
		{
			this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5f * (1f - (i / 100f));
			yield return new WaitForSeconds(0.01f);
		}
		this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5f * 0.85f;
	}
	public IEnumerator ZoomOut()
	{
		for (int i = 1; i <= 15; i++)
		{
			this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5f * (0.85f + (i / 100f));
			yield return new WaitForSeconds(0.01f);
		}
		this.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 5f;
	}
}
