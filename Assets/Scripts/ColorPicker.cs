using MagicKit;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    // Properties
    public ControllerInput Controller;
    public Slice SliceBehavior;
    public Renderer Blade;
    
    // State
    private RaycastHit[] _hits;
    private float _length;
    private GameObject _source;
    private SpriteRenderer _sprite;
    private string _name;
    
    void Start ()
    {
        _hits = new RaycastHit[8];
        _length = SliceBehavior.Length;
        _source = SliceBehavior.Source;
        _sprite = GetComponent<SpriteRenderer>();
        _name = gameObject.name;
    }
    
    void Update () 
    {
        var transform = _source.transform;
        var position = transform.position;
        var direction = transform.up;
		
        var hitCount = Physics.RaycastNonAlloc(position, direction, _hits, _length);
        if (hitCount == 0)
        {
            return;
        }

        for (var i = 0; i < hitCount; i++)
        {
            var hit = _hits[i];
            if (hit.collider.gameObject.name == _name)
            {
                Vector2 textureCoord = hit.textureCoord;
                var hitColor = _sprite.sprite.texture.GetPixel((int)(_sprite.sprite.texture.width * textureCoord.x), (int)(_sprite.sprite.texture.height * textureCoord.y));
                Debug.Log("Hit: " + textureCoord + " = " + hitColor);
                if (hitColor.a == 0) return;
                var bladeMaterial = Blade.material;
                bladeMaterial.color = hitColor;
                bladeMaterial.SetColor("_EMISSION", hitColor);
            }
        }
    }
}
