using UnityEngine;

//https://github.com/joscanper/unity_coneofsightfx
//[ExecuteAlways]
public class Script_ConeOfSightRenderer : MonoBehaviour
{
    private static readonly int sViewDepthTexturedID = Shader.PropertyToID("_ViewDepthTexture");
    private static readonly int sViewSpaceMatrixID = Shader.PropertyToID("_ViewSpaceMatrix");

    public bool m_RenderCone = true;

    public Camera m_ViewCamera;
    public float m_ScaledViewDistance;
    public float m_ViewDistance;
    public float m_ViewAngle;

    private Material m_Material;
    private MeshRenderer m_MeshRenderer;

    private void Start()
    {
        m_MeshRenderer = GetComponent<MeshRenderer>();
        //if (Application.isPlaying)
        //{
            //m_Material = renderer.material;  // This generates a copy of the material
            m_Material = new Material(m_MeshRenderer.material);
            m_MeshRenderer.material = m_Material;     
        //}
        //else
        //{
        //    m_Material = renderer.sharedMaterial; 
        //}
        RenderTexture depthTexture = new RenderTexture(m_ViewCamera.pixelWidth, m_ViewCamera.pixelHeight, 32, RenderTextureFormat.Depth);

        m_ViewCamera.depthTextureMode = DepthTextureMode.Depth;
        m_ViewCamera.SetTargetBuffers(depthTexture.colorBuffer, depthTexture.depthBuffer);

        m_ViewCamera.farClipPlane = m_ScaledViewDistance;
        m_ViewCamera.fieldOfView = m_ViewAngle;

        m_Material.SetTexture(sViewDepthTexturedID, depthTexture);
        m_Material.SetFloat("_ViewAngle", m_ViewAngle);

        transform.localScale = new Vector3(m_ViewDistance * 2, transform.localScale.y, m_ViewDistance * 2);
    }

    private void Update()
    {
        m_MeshRenderer.enabled = m_RenderCone;
        if (m_MeshRenderer.enabled)
        {
        //TODO to be removed for better performance once parameter adjustment, or just make sure this executes in Build environment, not in release.
        //#if UNITY_EDITOR
        //        if (!Application.isPlaying)
        //        {
        m_ViewCamera.farClipPlane = m_ScaledViewDistance;
        m_ViewCamera.fieldOfView = m_ViewAngle;
        m_Material.SetFloat("_ViewAngle", m_ViewAngle);
        //        }
        //#endif
            m_ViewCamera.Render();

            m_Material.SetMatrix(sViewSpaceMatrixID, m_ViewCamera.projectionMatrix * m_ViewCamera.worldToCameraMatrix);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, new Vector3(1f, 0f, 1f));
        Gizmos.DrawWireSphere(Vector3.zero, m_ScaledViewDistance);
        Gizmos.matrix = Matrix4x4.identity;
    }

#endif

}