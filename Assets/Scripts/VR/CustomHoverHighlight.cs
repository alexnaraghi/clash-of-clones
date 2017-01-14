using UnityEngine;

//-------------------------------------------------------------------------
public class CustomHoverHighlight : MonoBehaviour
{
    public Material highLightMaterial;

    private MeshRenderer bodyMeshRenderer;
    private MeshRenderer trackingHatMeshRenderer;
    private SteamVR_RenderModel renderModel;
    private bool renderModelLoaded = false;

    SteamVR_Events.Action renderModelLoadedAction;

    //-------------------------------------------------
    void Awake()
    {
        renderModelLoadedAction = SteamVR_Events.RenderModelLoadedAction( OnRenderModelLoaded );
    }


    //-------------------------------------------------
    void OnEnable()
    {
        renderModelLoadedAction.enabled = true;
    }


    //-------------------------------------------------
    void OnDisable()
    {
        renderModelLoadedAction.enabled = false;
    }


    //-------------------------------------------------
    public void Initialize( int deviceIndex )
    {
        renderModel = gameObject.AddComponent<SteamVR_RenderModel>();
        renderModel.SetDeviceIndex( deviceIndex );
        renderModel.updateDynamically = false;
    }


    //-------------------------------------------------
    private void OnRenderModelLoaded( SteamVR_RenderModel renderModel, bool success )
    {
        if ( renderModel != this.renderModel )
        {
            return;
        }

        Transform bodyTransform = transform.Find( "body" );
        if ( bodyTransform != null )
        {
            bodyMeshRenderer = bodyTransform.GetComponent<MeshRenderer>();
            bodyMeshRenderer.material = highLightMaterial;
            bodyMeshRenderer.enabled = false;
        }

        Transform trackingHatTransform = transform.Find( "trackhat" );
        if ( trackingHatTransform != null )
        {
            trackingHatMeshRenderer = trackingHatTransform.GetComponent<MeshRenderer>();
            trackingHatMeshRenderer.material = highLightMaterial;
            trackingHatMeshRenderer.enabled = false;
        }

        foreach ( Transform child in transform )
        {
            if ( ( child.name != "body" ) && ( child.name != "trackhat" ) )
            {
                Destroy( child.gameObject );
            }
        }

        renderModelLoaded = true;
    }

    //-------------------------------------------------
    public void ShowHighlight()
    {
        if ( renderModelLoaded == false )
        {
            return;
        }

        if ( bodyMeshRenderer != null )
        {
            bodyMeshRenderer.enabled = true;
        }

        if ( trackingHatMeshRenderer != null )
        {
            trackingHatMeshRenderer.enabled = true;
        }
    }


    //-------------------------------------------------
    public void HideHighlight()
    {
        if ( renderModelLoaded == false )
        {
            return;
        }

        if ( bodyMeshRenderer != null )
        {
            bodyMeshRenderer.enabled = false;
        }

        if ( trackingHatMeshRenderer != null )
        {
            trackingHatMeshRenderer.enabled = false;
        }
    }
}