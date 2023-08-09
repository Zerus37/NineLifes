using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	public UnityEvent<int> OnBulletChange = new UnityEvent<int>();

	[SerializeField] private Transform _head;
	[SerializeField] private Transform _body;
	[SerializeField] private Rigidbody2D _rb;
	[SerializeField] private float _speed = 5f;
	[SerializeField] private float _cooldown = 0.5f;
	[SerializeField] private float _reloadTime = 3f;
	[SerializeField] private float _damage = 5f;
	[SerializeField] private int _bulletMax = 18;
	[SerializeField] private Transform _hitStart;

	private Vector3 moveVector = Vector3.zero;
	private float _currentCooldown = 0f;
	private int _bulletCurrent;

	private float _targetRotateX = 0;

	private void Awake()
	{
		_bulletCurrent = _bulletMax;
		OnBulletChange.Invoke(_bulletCurrent);
	}

	//Этот апдейт надо вызывать из другого апдейта в монобехе, вызывается в GameState
	public void PausedUpdate()
	{
		if (_targetRotateX < 0)
		{
			_head.localRotation = Quaternion.Euler(Mathf.Lerp(_head.rotation.x, _targetRotateX*2, 0.25f), 0, 0);
			_targetRotateX = Mathf.Clamp(Mathf.Lerp(_targetRotateX, 0, Time.deltaTime*4)+0.0002f, -45f, 0f);
		}

		if (_currentCooldown > 0)
			_currentCooldown -= Time.deltaTime;
	}

	public bool Fire()
	{
		if (_currentCooldown <= 0 && _bulletCurrent > 0)
		{
			_bulletCurrent -= 1;
			_currentCooldown = _cooldown;

			OnBulletChange.Invoke(_bulletCurrent);

			if (_bulletCurrent <= 0)
				Invoke(nameof(Reload), _reloadTime);

			RaycastHit2D hit = Physics2D.Raycast(_hitStart.position, transform.forward);
			if (hit.collider != null)
				Debug.Log(hit.collider.gameObject.name);


			if (hit.collider != null && hit.collider.TryGetComponent<Life>(out Life victim))
			{
				victim.TakeDamage(_damage);
			}

			return true;
		}

		return false;
	}

	public void Move(Vector3 moveInput)
	{
		moveVector = transform.forward * moveInput.z;
		moveVector += transform.right * moveInput.x;
		moveVector.Normalize();

		_rb.velocity = _speed * moveVector;
	}

	public void RotateX(float value)
	{
		_targetRotateX += value;
	}

	public void RotateY(float value)
	{
		_body.Rotate(0, value, 0);
	}

	private void Reload()
	{
		_bulletCurrent = _bulletMax;
		OnBulletChange.Invoke(_bulletCurrent);
	}
}
