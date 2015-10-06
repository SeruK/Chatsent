using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using IMVU;

public class IMVULogin : LocalAssetLoader {
	private void OnEnable() {
		StartCoroutine( "Login" );
	}

	private void OnDisable() {
		StopCoroutine( "Login" );
	}

	private IEnumerator Login() {
		Imvu.Login().Then(
			userModel => Load( userModel )
		).Catch(
			err => Debug.LogError( err )
		);
		GameObject okButton = null;
		while( okButton == null ) {
			yield return new WaitForSeconds( 2.0f );
			okButton = GameObject.Find( "OK Button" );
		}
		SetInputField( "InputField Username", "gaythugdating" );
		SetInputField( "InputField Password", "1983JaksXn-1" );
		var pointer = new PointerEventData( EventSystem.current );
		ExecuteEvents.Execute( okButton, pointer, ExecuteEvents.submitHandler );		
	}

	private void SetInputField( string goName, string value ) {
		var go = GameObject.Find( goName );
		if( go != null ) {
			var field = go.GetComponent<InputField>();
			if( field != null ) {
				field.text = value;
			}
		}
	}
}
