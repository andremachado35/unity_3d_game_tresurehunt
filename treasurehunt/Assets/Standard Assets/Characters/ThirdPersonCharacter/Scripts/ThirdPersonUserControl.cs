using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class ThirdPersonUserControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        public Text countKeyText;

        public Text youlostText;

        private int countKeysInt;
        private Rigidbody rb;
        private bool trigger;
        private void Start()
        {
            trigger = false;
            rb = GetComponent<Rigidbody>();
            countKeysInt =0;
            setCountKeyText();
            youlostText.gameObject.SetActive(false);
            // get the transform of the main camera
            if (Camera.main != null)
            {
                m_Cam = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning(
                    "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
                // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
            }

            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
             if (rb.position.y >=-5 )
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");
            bool crouch = Input.GetKey(KeyCode.C);

            // calculate move direction to pass to character
            if (m_Cam != null)
            {
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v*m_CamForward + h*m_Cam.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                m_Move = v*Vector3.forward + h*Vector3.right;
            }
#if !MOBILE_INPUT
			// walk speed multiplier
	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
#endif

            // pass all parameters to the character control script
            m_Character.Move(m_Move, crouch, m_Jump);
            m_Jump = false;
        }
        else{
            StartCoroutine (ExecuteAfterTime("Tente Novamente!",3.0f));
             
            }
        }
    
    void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Key")){
            other.gameObject.SetActive(false);
            countKeysInt=countKeysInt+1;
            setCountKeyText();
        }
        
        if(other.gameObject.CompareTag("Bau")) {
            
            Console.WriteLine("bool:{0}",trigger);
            if(trigger!=false){
            other.gameObject.SetActive(false);
            StartCoroutine(ExecuteAfterTime("Parabéns! Você GANHOU!",8.0f));
            StartCoroutine (resetGame());        
            }
        }
    }
    void setCountKeyText()
    {   
        countKeyText.text = "Keys: " + countKeysInt.ToString()+"/4";
        if(countKeysInt==4){
            StartCoroutine(ExecuteAfterTime("Você pode ir abrir o cofre! PARABÉNS!!!",5.0f));
            trigger = true;
        }
        else{
            int x = 4 - countKeysInt;
            String mensgem = "Falta só " + x.ToString();
            StartCoroutine(ExecuteAfterTime(mensgem,3.0f));
            
        }
    }
    
     IEnumerator ExecuteAfterTime(string te, float time)
    {
        youlostText.gameObject.SetActive(true);
        youlostText.text = te;
        youlostText.enabled = true;
        yield return new WaitForSeconds(time);
        youlostText.enabled = false;

    }

    IEnumerator resetGame(){
        yield return new WaitForSeconds(10.0f);
        SceneManager.LoadScene("menu");
        countKeysInt = 0 ;
    }
}
}
