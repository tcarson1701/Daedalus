using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGameState
{
}

public class GameLoopScript : MonoBehaviour
{
	public static CGameState ms_GameState = new CGameState();



	public GameObject m_AiUnitPrefab;
	public GameObject m_PlayerUnitPrefab;

	// Use this for initialization
	void Start ()
	{
		GameObject playerUnit = ( GameObject )
			Instantiate ( m_PlayerUnitPrefab, new Vector3 (0, 6, 0), Quaternion.identity );
		UnitBirdBehaviorScript playerScript = playerUnit.GetComponent< UnitBirdBehaviorScript >();
		playerScript.SetPlayer();

		for ( int i = 0; i < 3; i++ )
		{
			GameObject aiUnit = ( GameObject )
				Instantiate ( m_AiUnitPrefab, new Vector3( i + 2, 6, 0), Quaternion.identity );
			UnitBirdBehaviorScript aiScript = aiUnit.GetComponent< UnitBirdBehaviorScript >();
			aiScript.SetAi();
			aiScript.SetEnemyUnit( playerUnit );
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
