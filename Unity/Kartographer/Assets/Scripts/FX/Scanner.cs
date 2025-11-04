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
        private List<Vector3> _positionsList = new();
        private List<VisualEffect> _vfxList = new();
        private VisualEffect _currentVFX;
        private Texture2D _texture;
        private Color[] _positions;
        private bool _createNewVFX;
        private int _particleAmount;

        private const string TEXTURE_NAME = "PositionTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";
        private const string PARTICLE_AMOUNT_PARAMETER_NAME = "ParticleAmount";

        [SerializeField] private int _particleCompleteScanAmount = 1000;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private VisualEffect _vfxPrefab;
        [SerializeField] public GameObject _vfxContainer;
        [SerializeField] private Transform _castPoint;
        [SerializeField] private float _radius = 1f;
        [SerializeField] private Transform _scannerLaserPoint;
        [SerializeField] private int _pointsPerScan = 1;
        [SerializeField] private float _range = 10f;
        [SerializeField] private int resolution = 50;
        [SerializeField] private InputActionReference toggleScannerAction;
        [SerializeField] private GameObject scannerGunGO;
        [SerializeField] private AudioClip scannerOnSound;
        [SerializeField] private AudioClip scannerOffSound;

        [HideInInspector] public bool isScanning = true;
        private Vector3 _lastHitPoint;
        private bool _hasLastHit = false;
        private bool allowedToScan = false;


        //timer
        [SerializeField] private float resetDelay = 5f; // seconds of inactivity before reset
        private float _inactivityTimer = 0f;

        private void Start()
        {
            _fire = playerInput.actions["Fire"];
            scannerGunGO.SetActive(allowedToScan);
            _lineRenderer.enabled = false;
            _createNewVFX = true;
            //CreateNewVisualEffect();
            //ApplyPositions();
        }

        private void Update()
        {
            if (toggleScannerAction.action.WasPressedThisFrame())
            {
                allowedToScan = !allowedToScan;
                scannerGunGO.SetActive(allowedToScan);
                if (allowedToScan) SoundManager.Instance.PlaySound2D(scannerOnSound, 0.5f);
                if (!allowedToScan) SoundManager.Instance.PlaySound2D(scannerOffSound, 0.3f);
            }
        }

        private void FixedUpdate()
        {
            if (_vfxContainer == null) return;
            if (_vfxContainer.GetComponent<VFXContainer>().buildingScanned) return;
            if (!allowedToScan) return;

            Scan();

            scannedPercentage = ScannedAmount();

            if (_fire.IsPressed())
                _inactivityTimer = 0f;
            else
                _inactivityTimer += Time.fixedDeltaTime;

            if (_inactivityTimer >= resetDelay)
            {
                ResetScan();
            }
        }

        public void RemoveFX()
        {
            _inactivityTimer = 0;
            GetComponentInParent<ScannerUI>().ResetUI();

            // Stop current scanning
            _lineRenderer.enabled = false;

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
        }

        public void ResetScan()
        {
            _inactivityTimer = 0;
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
            ApplyPositions();
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
            _currentVFX = Instantiate(_vfxPrefab, _vfxContainer.transform.position + Vector3.up * -5f, Quaternion.identity, _vfxContainer.transform);
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
            if (_fire.IsPressed() && scannedPercentage < 1f)
            {
                _currentVFX.enabled = true;

                for (int i = 0; i < _pointsPerScan; i++)
                {
                    Vector3 randomOffset = Random.insideUnitSphere * _radius;

                    if (Vector3.Dot(randomOffset, transform.forward) < 0f)
                        randomOffset = -randomOffset;

                    Vector3 randomPoint = _castPoint.position + randomOffset;
                    Vector3 dir = (randomPoint - transform.position).normalized;

                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, _range, _layerMask))
                    {
                        // store the last hit point for the line renderer
                        _lastHitPoint = hit.point;
                        _hasLastHit = true;

                        if (_positionsList.Count < resolution * resolution)
                        {
                            bool tooClose = false;
                            float minDistance = 0.5f;

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
                                _particleAmount++;
                                SoundManager.Instance.PlaySound2D(hitPointSFX, .2f);
                            }
                        }
                        else
                        {
                            _createNewVFX = true;
                            CreateNewVisualEffect();
                            break;
                        }
                    }
                }

                ApplyPositions();

                // Update line renderer if we hit something
                if (_hasLastHit)
                {
                    _lineRenderer.enabled = true;
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.SetPosition(0, _scannerLaserPoint.position); // base of gun
                    _lineRenderer.SetPosition(1, _lastHitPoint); // most recent hit
                }
                else _lineRenderer.enabled = false;
            }
            else if (scannedPercentage >= 1f && isScanning)
            {
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
}
