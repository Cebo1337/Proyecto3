using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSController : MonoBehaviour {

    //Float - Ammo - ReloadButton vs ReloadEmpty-  DoorAnim - Serie Dianas - IA - COlliders dron


    // Use this for initialization
    CharacterController m_CharacterController;


    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool m_InvertedYaw = false;
    public bool m_InvertedPitch = true;
    Vector3 l_Movement;
    bool moving = false;
    private bool inside = false;
    float m_VerticalSpeed = 0.0f;
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_JumpSpeed;
    public bool m_OnGround = false;
    //checkpoints
    public bool checkpoint1, checkpoint2, checkpoint3;
    // public Vector3 spawnPosition;

   public Image transparent;

    //Disparo
    public int m_MouseShootButton = 0;
   public int m_CurrentAmmoCount;
   public int m_MaxAmmo = 150;
    int m_ReloadAmmo = 30;
    public GameObject m_BulletHoleDecal;
    public GameObject m_ShootHitParticles;
    public GameObject m_ShootingParticle;
    public LayerMask m_ShootLayerMask;
    int Points = 0;
    //animaciones
    public Animation m_WeaponAnimation;
    public AnimationClip m_IdleWeaponAnimationClip;
    public AnimationClip m_ShootWeaponAnimationClip;
    public AnimationClip m_ReloadWeaponAnimationClip;

    //HP
    public float m_currentLife;
    public float m_totalLife = 100;
    public float m_Totalshield = 100;
    public float m_currentShield;
    //counters
    public int NumberofKeys = 0;
    public int NumerOfPoints = 0;
    public float distance;
    public bool inIceMap;
    //Audio
    public AudioSource m_Audio;
    public Renderer render;
    // float counter = 0;
    bool played = false;
  
    void Awake()
    {
        // m_currentShield = m_Totalshield;

        m_CharacterController = GetComponent<CharacterController>();
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;
        //   CPoolElements(25, m_BulletHoleDecal, m_DestroyObjects);
    }

    

    private void Start()
    {

        inIceMap = false;

            m_currentLife = m_totalLife;
            m_CurrentAmmoCount = m_ReloadAmmo;

     




        played = false;
    }
    void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
          //  m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
       

            //…
            float l_MouseAxisY = Input.GetAxis("Mouse Y");
        if (m_InvertedPitch)
        {
            l_MouseAxisY = -l_MouseAxisY;
        }

        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        float l_MouseAxisX = Input.GetAxis("Mouse X");
        if (m_InvertedYaw)
        {
            l_MouseAxisX = -l_MouseAxisX;
        }
        m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime;
       Vector3 l_Movement = Vector3.zero;

        //…
        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
        moving = false;
        if (Input.GetKey(m_UpKeyCode))
        {

            l_Movement = l_Forward;
            moving = true;
        }
        else if (Input.GetKey(m_DownKeyCode))
        {

            l_Movement = -l_Forward;
            moving = true;
        }

        if (Input.GetKey(m_RightKeyCode))
        {

            l_Movement += l_Right;
            moving = true;
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {

            l_Movement -= l_Right;
            moving = true;

        }
            l_Movement.Normalize();
        l_Movement *= Time.deltaTime * m_Speed;
        
        if (!moving)
        {
          //  l_Movement = Vector3.zero;
        }
       

        // CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        //…
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_OnGround = true;
            m_VerticalSpeed = 0.0f;
        }
        else
            m_OnGround = false;

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;
        //…
        float l_SpeedMultiplier = 1.0f;
        float m_FastSpeedMultiplier = 6.0f;
        if (Input.GetKey(m_RunKeyCode))
            l_SpeedMultiplier = m_FastSpeedMultiplier;
        //…
        l_Movement *= Time.deltaTime * m_Speed * l_SpeedMultiplier;
       m_CharacterController.Move(l_Movement);

        //…
        if (m_OnGround && Input.GetKeyDown(m_JumpKeyCode))
        {
           
            m_VerticalSpeed = m_JumpSpeed;
          
          
        }
        //if (m_CurrentAmmoCount == 0)
        //  Reload()
        if (Input.GetMouseButtonDown(m_MouseShootButton))
        {
            if (!m_WeaponAnimation["AKShoot"].enabled || m_WeaponAnimation["AKShoot"].weight < 0.1f  )
            {
                if(!m_WeaponAnimation["AKReload"].enabled || m_WeaponAnimation["AKReload"].weight < 0.1f)
                    Shoot();

            }

        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log(m_CurrentAmmoCount);
            //Debug.Log(m_MaxAmmo);
            if (!m_WeaponAnimation["AKShoot"].enabled || m_WeaponAnimation["AKShoot"].weight < 0.1f)
            {
                if (!m_WeaponAnimation["AKReload"].enabled || m_WeaponAnimation["AKReload"].weight < 0.1f)
                    Reload();
            }
        
        }
        if (inIceMap)
        {

            distance = -28;
        }
        else
        {

            distance = 30;
        }
        if (Input.GetMouseButtonDown(1))
        {

            if (!inIceMap)
            {
                
            StartCoroutine(FadeImage(false));
            StartCoroutine(FadeImage(true));

                this.gameObject.transform.position = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y+ distance, this.gameObject.transform.position.z);

                inIceMap = true;
            }
            else
            {
                StartCoroutine(FadeImage(false));
                StartCoroutine(FadeImage(true));
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + distance, transform.position.z);
                inIceMap = false;

            }
        }
        //…
    }

  
    void Reload()
    {
        if (m_MaxAmmo>0)
        {
           
            m_WeaponAnimation.CrossFade(m_ReloadWeaponAnimationClip.name);
          m_WeaponAnimation.CrossFadeQueued(m_IdleWeaponAnimationClip.name);
            m_CurrentAmmoCount = m_ReloadAmmo;
            m_MaxAmmo -= m_ReloadAmmo;

        }
              
        
    }


    void Shoot()
    {
        SetShootWeaponAnimation();
        m_CurrentAmmoCount--;
        GameObject.Instantiate(m_ShootingParticle, Camera.main.ViewportToWorldPoint(new Vector3(0.55f, 0.45f, 2f)), Quaternion.identity);
       
        Ray l_CameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
        {
           // Debug.Log(l_RaycastHit.transform.name);
           

             CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal, l_RaycastHit.transform.gameObject);
            
           
        
        }
        
    }
    /* void CreateShootHitParticles(Vector3 Position, Vector3 Normal)
     {
         GameObject.Instantiate(m_ShootHitParticles, Position, Quaternion.identity, m_GameController.m_DestroyObjects);
     }*/
    void CreateShootHitParticles(Vector3 Position, Vector3 Normal, GameObject parent)
    {
        //…
        // GameObject bullet = GameObject.Instantiate(m_BulletHoleDecal, Position, Quaternion.LookRotation(Normal), m_GameController.m_DestroyObjects);
        // bullet.transform.parent = parent.transform;
        GameObject bullet = Instantiate(m_BulletHoleDecal);
        if (bullet != null)
        {
            bullet.transform.parent = parent.transform;

            bullet.transform.position = Position;
            bullet.transform.rotation = Quaternion.LookRotation(Normal);
            if (!bullet.activeInHierarchy)
                bullet.SetActive(true);
        }

        

    }

    


    void SetIdleWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_IdleWeaponAnimationClip.name);
    }
    void SetShootWeaponAnimation()
    {
        m_WeaponAnimation.CrossFade(m_ShootWeaponAnimationClip.name);
        m_WeaponAnimation.CrossFadeQueued(m_IdleWeaponAnimationClip.name);
    }



    public IEnumerator FadeImage(bool fadeAway)
    {
        //  CR_isRunning = true;

        Color old;

        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {

                old = transparent.color;
               // render.material.color = new Color(old.r, old.g, old.b, i);
               transparent.color = new Color(old.r, old.g, old.b, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                old = transparent.color;

                //   render.material.color = new Color(old.r, old.g, old.b, i);
                transparent.color = new Color(old.r, old.g, old.b, i);

                yield return null;
            }
        }
        // yield return new WaitForSeconds(3.0f);

        //  CR_isRunning = false;

    }



}

