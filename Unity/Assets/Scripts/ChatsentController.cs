using UnityEngine;
using IMVU;

public class ChatsentController : LocalAssetLoader {
	private AssetInfo assetInfo = null;

	protected void OnEnable() {
		Imvu.Login().Then(
			userModel => Load( userModel )
		).Then(
			assetInfo => {
				this.assetInfo = assetInfo;
			}
		).Catch(
			err => Debug.LogError( err )
		);
	}
}
