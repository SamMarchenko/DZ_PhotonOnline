using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ProjectileController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField, Range(1f, 10f)] private float _moveSpeed = 3f;
    [SerializeField, Range(1f, 10f)] private float _damage = 1f;
    [SerializeField, Range(1f, 10f)] private float _lifetime = 20f;

    public float GetDamage => _damage;
    public string Parent { get; set; }
    
    
    void Start()
    {
        StartCoroutine(OnDie());
    }

 
    void Update()
    {
        transform.position += transform.forward * _moveSpeed * Time.deltaTime;
    }

    private IEnumerator OnDie()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ProjectileData.Create(this));
        }
        else
        {
            ((ProjectileData)stream.ReceiveNext()).Set(this);
        }
    }
}