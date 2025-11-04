/*
 * Author: Leonhard Robin Schnaitl
 * GitHub: https://github.com/leonhardrobin
*/
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LRS
{
    [RequireComponent(typeof(LineRenderer))]
    public class Scanner : MonoBehaviour
    {
        public float scannedPercentage = 0f;
        public AudioClip hitPointSFX;
        public AudioClip completeSound;
        private InputAction _fire;
        //private InputAction _changeRadius;
        private List<Vector3> _positionsList = new();
        private List<VisualEffect> _vfxList = new();
        private VisualEffect _currentVFX;
        private Texture2D _texture;
        private Color[] _positions;
        private bool _createNewVFX;
        private int _particleAmount;
        [SerializeField] private int _particleCompleteScanAmount = 1000;
        [SerializeField] private LineRenderer _lineRenderer;

        //private const string REJECT_LAYER_NAME = "PointReject";
        //private const string PLAYER_TAG = "Player";
        private const string TEXTURE_NAME = "PositionTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";
        private const string PARTICLE_AMOUNT_PARAMETER_NAME = "ParticleAmount";
        //private const string PARTICLES_PER_SCAN_PARAMETER_NAME = "ParticlesPerScan";

        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private VisualEffect _vfxPrefab;
        [SerializeField] public GameObject _vfxContainer;
        [SerializeField] private Transform _castPoint;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private float _maxRadius = 1f;
        [SerializeField] private float _minRadius = 0.5f;
        [SerializeField] private int _pointsPerScan = 1;
        [SerializeField] private float _range = 10f;

        [SerializeField] private int resolution = 50;

        [SerializeField] private float minParticleDistance = 0.5f; // in world units, x tresting density stuff
        public bool isScanning = true;

        //timer
        [SerializeField] private float resetDelay = 10f; // seconds of inactivity before reset
        private float _inactivityTimer = 0f;



        private void Start()
        {
            // Get InputAction from PlayerInput
            _fire = playerInput.actions["Fire"];
            //_changeRadius = playerInput.actions["Scroll"];
            //_lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
            _createNewVFX = true;
            //CreateNewVisualEffect();
            //ApplyPositions();
        }

        private void FixedUpdate()
        {
            Scan();
            scannedPercentage = ScannedAmount();
            //Debug.Log(scannedPercentage + "scanned percent");
            //ChangeRadius();

            // 🔹 Update inactivity timer
            if (_fire.IsPressed())
                _inactivityTimer = 0f;
            else
                _inactivityTimer += Time.fixedDeltaTime;

            if (_inactivityTimer >= resetDelay)
            {
                ResetScan();
            }
        }
        private void ResetScan()
        {
            //Debug.Log("Scanner reset due to inactivity.");
            GetComponentInParent<ScannerUI>().ResetUI();

            // Stop current scanning
            _lineRenderer.enabled = false;
            isScanning = true; // allow scanning again

            // Reset lists and data
            _positionsList.Clear();
            _particleAmount = 0;
            scannedPercentage = 0f;

            // Reset timer
            //_inactivityTimer = 0f;

            // Clear VFX
            if (_currentVFX != null)
                Destroy(_currentVFX.gameObject);

            // Clear all old effects
            foreach (var vfx in _vfxList)
                if (vfx != null) Destroy(vfx.gameObject);
            _vfxList.Clear();

            // Prepare a new VFX instance
            _createNewVFX = true;
            CreateNewVisualEffect();
            //_currentVFX.enabled = false;
        }

        public void ApplyPositions()
        {
            // create array from list
            Vector3[] pos = _positionsList.ToArray();

            // cache position for offset
            Vector3 vfxPos = _currentVFX.transform.position;

            // cache transform position
            Vector3 transformPos = transform.position;

            // cache some more stuff for faster access
            int loopLength = _texture.width * _texture.height;
            int posListLen = pos.Length;

            for (int i = 0; i < loopLength; i++)
            {
                Color data;

                if (i < posListLen - 1)
                {
                    data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
                }
                else
                {
                    data = new Color(0, 0, 0, 0);
                }
                _positions[i] = data;
            }

            // apply to texture
            _texture.SetPixels(_positions);
            _texture.Apply();

            // apply to VFX
            _currentVFX.SetTexture(TEXTURE_NAME, _texture);
            _currentVFX.Reinit();
        }

        public void CreateNewVisualEffect() // this is f***ing performance heavy help
        {
            // make sure it only gets called once
            if (!_createNewVFX) return;

            // add old VFX to list
            _vfxList.Add(_currentVFX);

            // create new VFX
            _currentVFX = Instantiate(_vfxPrefab, transform.position, Quaternion.identity, _vfxContainer.transform);
            _currentVFX.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);
            //_currentVFX.SetInt(PARTICLES_PER_SCAN_PARAMETER_NAME, _pointsPerScan);

            // create texture
            _texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);

            // create color array for positions
            _positions = new Color[resolution * resolution];

            // clear list
            _positionsList.Clear();

            // set particle amount to 0
            _particleAmount = 0;
            //_currentVFX.SetInt(PARTICLE_AMOUNT_PARAMETER_NAME, _particleAmount);

            _createNewVFX = false;
        }

        private void Scan()
        {
            // only call if button is pressed
            if (_fire.IsPressed() && scannedPercentage < 1f)
            {
                _currentVFX.enabled = true;
                for (int i = 0; i < _pointsPerScan; i++)
                {
                    Vector3 randomOffset = Random.insideUnitSphere * _radius;

                    // Flip if behind
                    if (Vector3.Dot(randomOffset, transform.forward) < 0f)
                    {
                        randomOffset = -randomOffset;
                    }

                    // Add cast point position
                    Vector3 randomPoint = _castPoint.position + randomOffset;

                    // Direction from scanner to point
                    Vector3 dir = (randomPoint - transform.position).normalized;

                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _range, _layerMask))
                    {
                        Debug.DrawRay(transform.position, dir * hit.distance, Color.green);

                        // Only add point if particle count limit is not reached
                        if (_positionsList.Count < resolution * resolution)
                        {
                            // --- START: minimum distance check ---
                            bool tooClose = false;
                            float minDistance = 0.5f; // set this to whatever world-space distance you want

                            foreach (var pos in _positionsList)
                            {
                                if (Vector3.Distance(pos, hit.point) < minDistance)
                                {
                                    tooClose = true;
                                    break;
                                }
                            }

                            if (!tooClose)
                            {
                                _positionsList.Add(hit.point);
                                _lineRenderer.enabled = true;
                                _lineRenderer.SetPositions(new[]
                                {
                            transform.position,
                            hit.point
                        });

                                _particleAmount++;
                                if (_particleAmount % 1 == 0)
                                {
                                    SoundManager.Instance.PlaySound2D(hitPointSFX, .2f);
                                }
                            }
                            // --- END: minimum distance check ---
                        }
                        else if (_fire.IsPressed())
                        {
                            _createNewVFX = true;
                            CreateNewVisualEffect();
                            break;
                        }


                    }
                } // for loop
                ApplyPositions();// button press
            }
            else if (scannedPercentage >= 1f && isScanning)
            {
                //SCAN COMPLETED
                SoundManager.Instance.PlaySound2D(completeSound, .3f);
                isScanning = false;
                GetComponentInParent<ScannerUI>().ScanComplete();
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }

        public float ScannedAmount()
        {
            if (_particleCompleteScanAmount == 0)
            { //Debug.Log("left scanned amount"); 
                return 0;
            }

            float percentage = (float)_particleAmount / _particleCompleteScanAmount;

            return percentage;
        }

    }
    // private void ChangeRadius()
    // {
    //     if (_changeRadius.triggered)
    //     {
    //         _radius = Mathf.Clamp(_radius + _changeRadius.ReadValue<float>() * Time.deltaTime, _minRadius, _maxRadius);
    //     }
    // }
}
