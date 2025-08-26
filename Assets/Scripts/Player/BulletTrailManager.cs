using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Weapons {
    public class BulletTrailHandler : MonoBehaviour {
        [SerializeField] private TrailRenderer _bulletTrailPrefab;
        [SerializeField] private float _fakeBulletSpeed = 500f;

        private ObjectPool<TrailRenderer> _trailPool;
        private WaitForSeconds _trailDelay;

        private void Awake() {
            var go = new GameObject("BulletTrails") {
                transform = {
                    position = Vector3.zero
                }
            };
            go.transform.SetParent(null);
            _trailPool = new ObjectPool<TrailRenderer>(
                () => Instantiate(_bulletTrailPrefab, go.transform),
                trail => {
                    trail.transform.parent = null;
                }, trail => {
                    trail.enabled =  false;
                    trail.transform.parent = go.transform;
                }, DestroyTrailData);

            _trailDelay = new WaitForSeconds(_bulletTrailPrefab.time);
        }

        public void AttachTrail(Transform spawnLocation, RaycastHit? hit) {
            var trail = _trailPool.Get();
            trail.transform.position = spawnLocation.position;
            trail.transform.rotation = Quaternion.identity;
            // https://www.reddit.com/r/Unity3D/comments/fhn7p8/comment/fkdwetn/
            // https://discussions.unity.com/t/591036
            trail.Clear();

            trail.enabled = true;

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (hit.HasValue) {
                StartCoroutine(SpawnTrail(trail, hit.Value.point));
            } else {
                StartCoroutine(SpawnTrail(trail, spawnLocation.position + spawnLocation.transform.forward * 100));
            }
        }

        private IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint) {
            var startPosition = trail.transform.position;
            var distance = Vector3.Distance(trail.transform.position, hitPoint);
            var remainingDistance = distance;
            while (remainingDistance > 0) {
                trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
                remainingDistance -= _fakeBulletSpeed * Time.deltaTime;
                yield return null;
            }
            trail.transform.position = hitPoint;
            // baked on awake
            yield return _trailDelay;
            _trailPool.Release(trail);
        }


        private static void DestroyTrailData(TrailRenderer trail) {
            Destroy(trail.gameObject);
        }
    }
}