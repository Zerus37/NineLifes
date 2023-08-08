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
	private float _yVelosity = 0;

	private void Awake()
	{
		_bulletCurrent = _bulletMax;
		OnBulletChange.Invoke(_bulletCurrent);
	}

	private void Update()
	{
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

			RaycastHit hit;
			if (Physics.Raycast(_hitStart.position, _hitStart.forward, out hit, Mathf.Infinity, 1))
			{
				if (hit.collider.TryGetComponent<Life>(out Life victim))
				{
					victim.TakeDamage(_damage);
				}
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

		//moveVector.y = _yVelosity;

		_rb.velocity = _speed * moveVector;
	}

	public void RotateX(float value)
	{
		_head.Rotate(value, 0, 0);
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
