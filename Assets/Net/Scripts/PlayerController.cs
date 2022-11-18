using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Controls _controls;
    private Transform _bulletPool;


    [SerializeField] private Transform _target;
    [SerializeField] private ProjectileController _bulletPrefab;

    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private PhotonView _photonView;

    //[SerializeField] private CapsuleCollider _collider;
    [SerializeField, Range(1f, 10f)] private float _moveSpeed = 2f;
    [SerializeField, Range(0.5f, 5f)] private float _maxSpeed = 2f;
    [SerializeField, Range(0.1f, 1f)] private float _attackDelay = 2f;
    [SerializeField, Range(0.1f, 1f)] private float _rotateDelay = 0.25f;


    [Range(1f, 50f)] public float Health = 5f;

    [SerializeField] private Transform _firePoint;


    // Start is called before the first frame update
    void Start()
    {
        _bulletPool = FindObjectOfType<EventSystem>().transform;

        _controls = new Controls();
        
        FindObjectOfType<GameManager>().AddPlayer(this);
        
    }

    private IEnumerator Fire()
    {
        while (true)
        {
            var bullet = PhotonNetwork.Instantiate(_bulletPrefab.name, _firePoint.position, transform.rotation).GetComponent<ProjectileController>();
            bullet.Parent = name;
            yield return new WaitForSeconds(_attackDelay);
        }
    }

    private IEnumerator Focus()
    {
        while (true)
        {
            transform.LookAt(_target);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

            yield return new WaitForSeconds(_rotateDelay);
        }
    }

    void FixedUpdate()
    {
        if (!_photonView.IsMine) return;

        var direction = _controls.Player1.Movement.ReadValue<Vector2>();

        if (direction.x == 0 && direction.y == 0)
        {
            return;
        }

        var velocity = _rigidbody.velocity;
        velocity += new Vector3(direction.x, 0, direction.y) * _moveSpeed * Time.deltaTime;

        velocity.y = 0f;
        velocity = Vector3.ClampMagnitude(velocity, _maxSpeed);
        _rigidbody.velocity = velocity;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_firePoint.position, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        var bullet = other.GetComponent<ProjectileController>();
        if (bullet == null || bullet.Parent == name) return;

        Health -= bullet.GetDamage;
        Destroy(other.gameObject);

        if (Health <= 0f)
        {
            Debug.Log($" {name} is dead!!!");
        }
    }

    private void OnDestroy()
    {
        _controls.Player1.Disable();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayerData.Create(this));
        }
        else
        {
            ((PlayerData)stream.ReceiveNext()).Set(this);
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;

        if (!_photonView.IsMine) return;
        
        _controls.Player1.Enable();

        StartCoroutine(Fire());
        StartCoroutine(Focus());
    }
}