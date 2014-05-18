using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitBirdBehaviorScript : MonoBehaviour
{
	//-----------------------------------------------------------------------------

	// Input
	public KeyCode m_keyFlap;
	public KeyCode m_keyDive;
	public KeyCode m_keyRight;
	public KeyCode m_keyLeft;

	// Properties
	public bool m_bIsPlayer;
	public float m_flLastFlapTime;
	public float m_flGravityScaleBase;
	public float m_flGravityScaleDive;
	
	// Shared Capabilities
	public float m_flFlapVerticalImpulse;
	public float m_flFlapHorizontalImpulse;
	public float m_flMaxSpeedHorizonal;

	// Player-specific
	public float m_flCameraOffsetMax;

	// AI-specific properties
	static public int ms_nAiIdNext;
	public int m_nAiId;
	public GameObject m_EnemyUnit;

	static public List< GameObject > ms_listAiUnits = new List< GameObject >();

	// AI-specific capabilities
	public float m_flFlapPeriodMin;
	public float m_flAiOffsetThreshold;
	public float m_flAiDiveThreshold;

	// AI-PD
	public float m_flAiHorizThreshold;
	public float m_flAiHorizPD_P;
	public float m_flAiHorizPD_D;

	//-----------------------------------------------------------------------------

	// External interface

	public void SetPlayer()
	{
		m_bIsPlayer = true;
	}

	public void SetAi()
	{
		m_bIsPlayer = false;
		m_nAiId = ms_nAiIdNext++;
		if ( !ms_listAiUnits.Contains( gameObject ) )
		{
			print ( "adding ai unit");
			ms_listAiUnits.Add( gameObject );
		}
	}

	public void SetEnemyUnit( GameObject enemyUnit )
	{
		m_EnemyUnit = enemyUnit;
	}

	//-----------------------------------------------------------------------------
	
	// Update is called once per frame
	void Update()
	{
		rigidbody2D.gravityScale = m_flGravityScaleBase;

		if ( m_bIsPlayer )
		{
			UpdatePlayer();
		}
		else
		{
			UpdateAi();
		}

		UpdateBounce();
	}

	void UpdateAi()
	{
		if ( Time.time > m_flLastFlapTime + m_flFlapPeriodMin )
		{
			float flHorizOutput = 0.0f;
			float flVertOutput = 0.0f;
			UpdateAiAttackController( m_EnemyUnit, ref flHorizOutput, ref flVertOutput );

			if ( flVertOutput > 0.0 )
			{
				bool bLeft = flHorizOutput < -m_flAiHorizThreshold;
				bool bRight = flHorizOutput > m_flAiHorizThreshold;
				ActionFlap( bLeft, bRight );
			}

			if ( flVertOutput < -m_flAiDiveThreshold )
			{
				ActionDive();
			}
		}
	}

	void UpdateAiAttackController( GameObject targetObject, ref float flHorizOutput_Out, ref float flVertOutput_Out )
	{
		float flVerticalError = 0.0f;
		float flHorizError = 0.0f;
		float flHorizErrorDt = 0.0f;

		if ( targetObject )
		{
			Vector2 vMyPos = transform.position;
			Vector2 vTargetPos = targetObject.transform.position;

			Vector2 vMyVel = rigidbody2D.velocity;
			Vector2 vTargetVel = targetObject.rigidbody2D.velocity;

			// Horizontal error, proportional
			flHorizError = ( vTargetPos.x - vMyPos.x );

			// Horizontal error, derivitive
			flHorizErrorDt = ( vTargetVel.x - vMyVel.x );

			// Horizontal error, proportional
			flVerticalError = ( vTargetPos.y - vMyPos.y );
		}

		flVertOutput_Out = flVerticalError;
		flHorizOutput_Out = flHorizError * m_flAiHorizPD_P + flHorizErrorDt * m_flAiHorizPD_D;
	}

	void UpdateAiAvoidController( List< GameObject >avoidList )
	{
	}

	void UpdateBounce()
	{
		Vector2 vPos = transform.position;
		Vector2 vVel = rigidbody2D.velocity;
		if ( vPos.y < 0.0f && vVel.y < 0.0f )
		{
			vPos.y = 0.0f;
			transform.position = vPos;
			vVel.y *= -0.8f;
			rigidbody2D.velocity = vVel;
		}
	}

	void ActionFlap( bool bLeft, bool bRight )
	{
		m_flLastFlapTime = Time.time;

		Vector2 vVel = rigidbody2D.velocity;
		vVel.y += m_flFlapVerticalImpulse;
		if ( bRight )
		{
			vVel.x += m_flFlapHorizontalImpulse;
		}
		else if ( bLeft )
		{
			vVel.x -= m_flFlapHorizontalImpulse;
		}
		Mathf.Clamp ( vVel.x, -m_flMaxSpeedHorizonal, m_flMaxSpeedHorizonal );
		rigidbody2D.velocity = vVel;
	}

	void ActionDive()
	{
		rigidbody2D.gravityScale = m_flGravityScaleDive;
	}

	void UpdatePlayer()
	{
		if ( Input.GetKeyDown ( m_keyFlap ) )
		{
			bool bLeft = Input.GetKey ( m_keyLeft );
			bool bRight = Input.GetKey ( m_keyRight );
			ActionFlap ( bLeft, bRight );
		}

		if ( Input.GetKey ( m_keyDive ) )
		{
			ActionDive ();
		}

		// Track camera
		{
			Vector2 vPos = transform.position;
			Vector3 vCameraPos = Camera.main.transform.position;
			vCameraPos.x = Mathf.Clamp ( vCameraPos.x, vPos.x - m_flCameraOffsetMax, vPos.x + m_flCameraOffsetMax );
			Camera.main.transform.position = vCameraPos;
		}
	}
}
