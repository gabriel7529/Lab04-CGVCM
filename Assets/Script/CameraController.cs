using UnityEngine;

public class VehicleCameraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform vehicleTransform; // El transform del vehículo
    public Camera mainCamera;
    
    [Header("Configuración de Cámaras")]
    public CameraSettings[] cameraSettings;
    
    [Header("Controles")]
    public KeyCode switchCameraKey = KeyCode.C;
    
    private int currentCameraIndex = 0;
    private Vector3 currentVelocity;
    private float currentRotationVelocity;
    
    [System.Serializable]
    public class CameraSettings
    {
        public string name;
        public Vector3 offset; // Posición relativa al vehículo
        public Vector3 rotation; // Rotación de la cámara
        public float followSpeed = 5f;
        public float rotationSpeed = 5f;
        public float fieldOfView = 60f;
        public bool lookAtVehicle = false; // Si debe mirar siempre al vehículo
        public float height = 2f; // Altura adicional
    }
    
    void Start()
    {
        if (vehicleTransform == null)
            vehicleTransform = GetComponent<Transform>();
            
        if (mainCamera == null)
            mainCamera = Camera.main;
            
        // Configurar cámaras por defecto si no hay ninguna
        if (cameraSettings == null || cameraSettings.Length == 0)
        {
            SetupDefaultCameras();
        }
        
        // Aplicar la primera cámara
        ApplyCamera(currentCameraIndex);
    }
    
    void Update()
    {
        // Cambiar cámara con tecla
        if (Input.GetKeyDown(switchCameraKey))
        {
            SwitchToNextCamera();
        }
        
        // Actualizar posición y rotación de la cámara
        UpdateCamera();
    }
    
    void SetupDefaultCameras()
    {
        cameraSettings = new CameraSettings[]
        {
            // Cámara trasera (tercera persona)
            new CameraSettings
            {
                name = "Tercera Persona",
                offset = new Vector3(0, 3, -6),
                rotation = new Vector3(10, 0, 0),
                followSpeed = 5f,
                rotationSpeed = 3f,
                fieldOfView = 60f,
                lookAtVehicle = false,
                height = 0f
            },
            
            // Cámara primera persona (cockpit)
            new CameraSettings
            {
                name = "Primera Persona",
                offset = new Vector3(0, 1.2f, 0.5f),
                rotation = new Vector3(0, 0, 0),
                followSpeed = 10f,
                rotationSpeed = 10f,
                fieldOfView = 75f,
                lookAtVehicle = false,
                height = 0f
            },
            
            // Cámara cenital
            new CameraSettings
            {
                name = "Vista Aérea",
                offset = new Vector3(0, 15, -5),
                rotation = new Vector3(60, 0, 0),
                followSpeed = 3f,
                rotationSpeed = 2f,
                fieldOfView = 50f,
                lookAtVehicle = true,
                height = 0f
            },
            
            // Cámara lateral
            new CameraSettings
            {
                name = "Vista Lateral",
                offset = new Vector3(8, 2, 0),
                rotation = new Vector3(0, 270, 0),
                followSpeed = 4f,
                rotationSpeed = 4f,
                fieldOfView = 60f,
                lookAtVehicle = true,
                height = 0f
            }
        };
    }
    
    void SwitchToNextCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameraSettings.Length;
        ApplyCamera(currentCameraIndex);
        
        // Mostrar nombre de la cámara (opcional)
        Debug.Log("Cámara: " + cameraSettings[currentCameraIndex].name);
    }
    
    public void SwitchToCamera(int index)
    {
        if (index >= 0 && index < cameraSettings.Length)
        {
            currentCameraIndex = index;
            ApplyCamera(currentCameraIndex);
        }
    }
    
    void ApplyCamera(int index)
    {
        var settings = cameraSettings[index];
        mainCamera.fieldOfView = settings.fieldOfView;
    }
    
    void UpdateCamera()
    {
        if (vehicleTransform == null || mainCamera == null) return;
        
        var settings = cameraSettings[currentCameraIndex];
        
        // Calcular posición objetivo
        Vector3 targetPosition = vehicleTransform.position + 
                               vehicleTransform.TransformDirection(settings.offset) + 
                               Vector3.up * settings.height;
        
        // Suavizar movimiento de la cámara
        mainCamera.transform.position = Vector3.SmoothDamp(
            mainCamera.transform.position, 
            targetPosition, 
            ref currentVelocity, 
            1f / settings.followSpeed
        );
        
        // Manejar rotación
        if (settings.lookAtVehicle)
        {
            // Mirar hacia el vehículo
            Vector3 lookDirection = vehicleTransform.position - mainCamera.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation, 
                targetRotation, 
                settings.rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Seguir la rotación del vehículo con offset
            Quaternion targetRotation = vehicleTransform.rotation * Quaternion.Euler(settings.rotation);
            mainCamera.transform.rotation = Quaternion.Slerp(
                mainCamera.transform.rotation, 
                targetRotation, 
                settings.rotationSpeed * Time.deltaTime
            );
        }
    }
    
    // Método para obtener información de la cámara actual
    public string GetCurrentCameraName()
    {
        return cameraSettings[currentCameraIndex].name;
    }
    
    public int GetCurrentCameraIndex()
    {
        return currentCameraIndex;
    }
    
    // Método para configurar follow dinamicamente
    public void SetFollowTarget(Transform newTarget)
    {
        vehicleTransform = newTarget;
    }
}

