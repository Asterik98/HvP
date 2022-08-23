using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;
    private PlayerMotor motor;
    float currentStopwatch=0f;
    public Text infoText;
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        float _xMov = Input.GetAxis("Horizontal");
        float _zMov = Input.GetAxis("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;
        motor.Move(_velocity);

        float _yRot = Input.GetAxisRaw("Mouse X");

        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;

        motor.Rotate(_rotation);

        float _xRot = Input.GetAxisRaw("Mouse Y");

        Vector3 _cameraRotation = new Vector3(_xRot, 0f, 0f) * lookSensitivity;

        motor.RotateCamera(_cameraRotation);
    }
    void PlayerStopwatch()
    {
        currentStopwatch += Time.deltaTime;
        string minutes = Mathf.Floor(currentStopwatch / 60).ToString("00");
        string seconds = (currentStopwatch % 60).ToString("00");
        string miliseconds = ((currentStopwatch * 10) % 10).ToString("00");
        string tempTimer = string.Format("{0}:{1}:{2}", minutes, seconds, miliseconds);
        infoText.text = tempTimer;
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
