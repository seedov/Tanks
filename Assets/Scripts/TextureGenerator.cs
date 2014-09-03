using UnityEngine;
using System.Collections;

public class TextureGenerator : MonoBehaviour {

    void Start () {
        //Sprite.Create(texture, new Rect(0, 0, 512, 512), Vector3.zero, 1);

	}
	

    void Update () {
        var hitinfo = new RaycastHit();
        var hit = Physics.Raycast(transform.position, Vector2.right, out hitinfo);
        print(hitinfo.point);


        var texture = new Texture2D(128, 128);
        var pixels = new Color[128 * 128];
        for (var i = 0; i < 128 * 128; ++i)
            pixels[i] = Color.red;
        texture.SetPixels(pixels);


        texture.SetPixel((int)hitinfo.point.x, (int)hitinfo.point.z, Color.black);
        renderer.material.mainTexture = texture;

        texture.Apply();
	
	}
}
