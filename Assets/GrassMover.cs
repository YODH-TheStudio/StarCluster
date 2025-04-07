using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassMover : MonoBehaviour
{
    private PlayerScript _player;
    [SerializeField] private Material grassShader;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameManager.Instance.GetPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (_player != null && grassShader != null) {
            Vector4 playerPos = new Vector4(_player.transform.position.x, _player.transform.position.y, _player.transform.position.z, 0);
            grassShader.SetVector("_PlayerPos", playerPos);
        }
    }
}
