using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    //skyrim kamera style - přepínání mezi FPS a TPS pohledem, detekce stěn, aby kamera nebyla ve zdi
    //todle je paradoxně nejtěžší část, protože musíme detekovat stěny a přizpůsobit vzdálenost kamery, aby nebyla ve zdi
    //pomahal mi gemini jinak bych nad tim stravil staři

    public Transform target; 
    public Transform cameraTransform;

    private float currentDist = 0f;
    public float fpsDist = 0.1f;
    public float tpsDist = 4.0f;
    public float smoothSpeed = 10f;

    private bool isFPS = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isFPS = !isFPS;
        }

        // přepínání kolečkem (stejné jako si to měl předtím)
        if (Input.GetAxis("Mouse ScrollWheel") < 0) isFPS = false;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) isFPS = true;

        float targetDist = isFPS ? fpsDist : tpsDist;

        // --- DETEKCE STĚN (Skyrim styl) ---
        RaycastHit hit;
        Vector3 direction = -cameraTransform.forward;
        if (Physics.Raycast(target.position + Vector3.up * 1.2f, direction, out hit, targetDist))
        {
            targetDist = hit.distance - 0.5f;
        }

        currentDist = Mathf.Lerp(currentDist, targetDist, Time.deltaTime * smoothSpeed);
        cameraTransform.position = (target.position + Vector3.up * 1.2f) - (cameraTransform.forward * currentDist);
    }
}