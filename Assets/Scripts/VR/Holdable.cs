using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;

[RequireComponent( typeof( Interactable ) )]
public class Holdable : MonoBehaviour 
{
    public bool CanDrop;
    public Material HighlightMaterial;
    [EnumFlags] public Hand.AttachmentFlags AttachmentFlags = Hand.defaultAttachmentFlags;
    public string AttachPoint;

    private Interactable _interactable;
    
    // Transform information for before the game object was grabbed.
    // We used this to return it to it's original slot after being ungrabbed.
    private Vector3 _originalLocalPosition;
    private Quaternion _originalLocalRotation;
    private Dictionary<MeshRenderer, Material[]> _preHighlightMaterials;

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();
    }

    //-------------------------------------------------
    // Called every Update() while a Hand is hovering over this object
    //-------------------------------------------------
    private void HandHoverUpdate( Hand hand )
    {
        if ( hand.GetStandardInteractionButtonDown())
        {
            if ( hand.currentAttachedObject != gameObject )
            {
                // Save our position/rotation so that we can restore it when we detach
                _originalLocalPosition = transform.localPosition;
                _originalLocalRotation = transform.localRotation;

                // Call this to continue receiving HandHoverUpdate messages,
                // and prevent the hand from hovering over anything else
                hand.HoverLock( _interactable );

                // Attach this object to the hand
                hand.AttachObject( gameObject, AttachmentFlags, AttachPoint );
            }
            else
            {
                if(CanDrop)
                {
                    // Detach this object from the hand
                    hand.DetachObject( gameObject );

                    // Call this to undo HoverLock
                    hand.HoverUnlock( _interactable );

                    // Restore position/rotation
                    transform.localPosition = _originalLocalPosition;
                    transform.localRotation = _originalLocalRotation;
                }
            }
        }
    }

    private void OnHandHoverBegin()
    {
        if(HighlightMaterial != null)
        {
            _preHighlightMaterials.Clear();
            foreach(var mesh in GetComponentsInChildren<MeshRenderer>())
            {
                _preHighlightMaterials.Add(mesh, mesh.materials);
                Material[] highlightMaterials = new Material[mesh.materials.Length];
                for(int i = 0; i < highlightMaterials.Length; i++)
                {
                    highlightMaterials[i] = HighlightMaterial;
                }
            }
        }
    }


    //-------------------------------------------------
    private void OnHandHoverEnd()
    {
        if (HighlightMaterial != null)
        {
            foreach (var meshKV in _preHighlightMaterials)
            {
                meshKV.Key.materials = meshKV.Value;
            }
            _preHighlightMaterials.Clear();
        }
    }
}
