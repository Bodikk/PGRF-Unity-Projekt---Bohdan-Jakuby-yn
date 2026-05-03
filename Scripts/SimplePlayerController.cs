using UnityEngine;
using UnityEngine.Audio;

public class SimplePlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Camera playerCamera;
    public float speed = 10f;
    public float mouseSensitivity = 300f; //senska 
    public float gravity = -9.81f; // gravitace 

    private float xRotation = 0f;
    private Vector3 velocity; // zbytečny vektor pro gravitaci, ale je to standardní způsob, jak to dělat v Unity, takže jsem to nechal, i když se tam moc nevyužívá, protože gravitace je konstantní a není potřeba ji měnit
    private bool isGrounded; // kontrola dotyku s podlahou
    public AudioSource audioSource;
    public AudioClip coinSound;
    public AudioClip trapSound;
    public AudioClip finishSound;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // kontrola, jestli stojíme na zemi (zabudovaná v Character Controlleru)
        isGrounded = controller.isGrounded;

        // pokud jsme na zemi a gravitace nás táhne dál dolů, trochu to přibrzdíme
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // 1. ROZHLÍŽENÍ MYŠÍ
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 2. CHŮZE
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 3. APLIKACE GRAVITACE
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // kontrolujeme, jestli se objekt jmenuje "coin"
        if (other.gameObject.name == "coin" || other.CompareTag("Bonus"))
        {
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
              //+10sec
                gm.timeLeft += 10f;

              
                GameManager.totalScore += 100; 
                Debug.Log("Skóre: " + GameManager.totalScore);
               
                //sound
                if (audioSource != null && coinSound != null)
                    audioSource.PlayOneShot(coinSound);
            }
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Trap"))
        {
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                //-5sec
                gm.timeLeft -= 5f;
                Debug.Log("past -5s");
                //sound
                if (audioSource != null && trapSound != null)
                    audioSource.PlayOneShot(trapSound);
            }
            Destroy(other.gameObject);
        }
        //cil
        if (other.CompareTag("Finish"))
        {
            // zvedne level v paměti a zapne vítěznou obrazovku
            GameManager gm = FindObjectOfType<GameManager>();
            if (gm != null)
            {
                if (audioSource != null && finishSound != null)
                    audioSource.PlayOneShot(finishSound);


                gm.WinGame();
            }
        }
    }
}