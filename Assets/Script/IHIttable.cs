using UnityEngine;

public interface IHittable
{
   void OnHit(Vector2 hitDirection);
}