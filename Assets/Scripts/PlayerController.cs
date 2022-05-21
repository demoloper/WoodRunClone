using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Current;
    public float limitX;

    public float xspeed;
    public float runningSpeed;
    private float _currentRunningSpeed;

    public GameObject ridingCylinderPrefab;
    public List<RidingCylinder> cylinders;

    private bool _spawingBridge;
    public GameObject bridgePiecePrefab;
    private BridgeSpawner _bridgeSpawner;

    private float _creatingBridgeTimer;

    private bool _finished;
    private float _scoreTimer = 0;
    public Animator animator;
    private float _lastTouchedX;
    private float _dropSoundTimer;

    public AudioSource cylinderAudioSource,triggerAudioSource,itemAudioSource;
    public AudioClip gatherAudioClip, dropAudioClip,coinAudioClip,buyAudipClip,equipItemAudioClip, unequipItemAudioClip;

    public List<GameObject> wearSpots;

   
   

  
    void Update()
    {
        if (LevelController.Current==null||!LevelController.Current.gameActive)
        {
            return;
        }
        float newX = 0;
        float toucXDelta = 0;
        if (Input.touchCount > 0)
        {
            if ( Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchedX = Input.GetTouch(0).position.x;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                toucXDelta = 5*(_lastTouchedX- Input.GetTouch(0).position.x) / Screen.width;
                _lastTouchedX = Input.GetTouch(0).position.x;

            }
            toucXDelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        else if(Input.GetMouseButton(0))
        {
            toucXDelta = Input.GetAxis("Mouse X");
        }

        newX = transform.position.x + xspeed * toucXDelta * Time.deltaTime;
        newX = Mathf.Clamp(newX, -limitX, limitX);


        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z+_currentRunningSpeed*Time.deltaTime);
        transform.position = newPosition;
        if (_spawingBridge)
        {
            PlayDropSound();
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer<0)
            {
                _creatingBridgeTimer = 0.01f;
                IncrementCylinderVolume(-0.01f);
                GameObject createdBridgePiece = Instantiate(bridgePiecePrefab);
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                createdBridgePiece.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                createdBridgePiece.transform.position = newPiecePosition;

                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer<0)
                    {
                        _scoreTimer = 0.1f;
                        LevelController.Current.ChangeScore(1);
                    }



                }


            }

        }

    }
    public void ChangeSpeed(float value) 
    {
        _currentRunningSpeed = value;
        
    
    
    }





    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="AddCylinder")
        {
            cylinderAudioSource.PlayOneShot(gatherAudioClip,0.1f);
            IncrementCylinderVolume(0.1f);
            Destroy(other.gameObject);
        }
        else if (other.tag=="SpawBridge")
        {
            StartSpawingBridge(other.transform.parent.GetComponent<BridgeSpawner>());


        }
        else if (other.tag=="StopSpawBridge")
        {
            StopSpawningBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }
        else if (other.tag=="Finish")
        {
            _finished = true;
            StartSpawingBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }
        else if (other.tag=="Coin")
        {
            triggerAudioSource.PlayOneShot(coinAudioClip, 0.1f);
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }

   


    }
    private void OnTriggerStay(Collider other)
    {
        if (LevelController.Current.gameActive)
        {       
            if (other.tag=="Trap")
            {
                PlayDropSound();
                IncrementCylinderVolume(-Time.fixedDeltaTime);

            }
        }
    }
    public void IncrementCylinderVolume(float value) 
    {
        if (cylinders.Count==0)
        {
            if (value>0)
            {
                CreateCylinder(value);
            }
            else if (value<0)
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();
                }
                else
                {
                    Die();
                }
            }
        }
        else
        {
            cylinders[cylinders.Count - 1].IncrementCylinderVolume(value);
        }
    }
    public void Die()
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6;
        Camera.main.transform.SetParent(null);
        LevelController.Current.GameOver();
    }
    public void CreateCylinder(float value)
    {
        RidingCylinder createdCylinder = Instantiate(ridingCylinderPrefab, transform).GetComponent<RidingCylinder>();
        cylinders.Add(createdCylinder);
        createdCylinder.IncrementCylinderVolume(value);
    }
    public void DestroyCylinder(RidingCylinder cylinder) 
    {
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);
    

    }
    public void StartSpawingBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawingBridge = true;

    
    }
    public void StopSpawningBridge()
    {
        _spawingBridge = false;

    }
    
    public void PlayDropSound() 
    {
        _dropSoundTimer -= Time.deltaTime;
        if (_dropSoundTimer<0)
        {
            _dropSoundTimer = 0.15f;
            cylinderAudioSource.PlayOneShot(dropAudioClip, 0.1f);

      
        }
    }



}
