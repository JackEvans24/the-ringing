using UnityEngine;

public static class RingHelpers
{
    public static (bool withinBounds, Vector2 point) GetPointWithinBounds(Rect bounds, Vector2 target, float border = 0f)
    {
        if (bounds.Contains(target))
            return (true, target - bounds.center);

        //var smallerDimension = Mathf.Min(bounds.width, bounds.height);
        var direction = target - bounds.center;

        //return bounds.center + (direction.normalized * smallerDimension / 2);

        var angleToCorner = Vector2.Angle(Vector2.up, bounds.max - bounds.center);
        var angleToTarget = Vector2.SignedAngle(Vector2.up, direction);
        var cosineAngle = angleToTarget;

        var adjacentMagnitude = bounds.height / 2;

        if (angleToTarget < -angleToCorner && angleToTarget >= (angleToCorner - 180))
        {
            adjacentMagnitude = bounds.width / 2;
            cosineAngle = Vector2.Angle(Vector2.right, direction);
        }
        else if (angleToTarget > angleToCorner && angleToTarget <= (180 - angleToCorner))
        {
            adjacentMagnitude = bounds.width / 2;
            cosineAngle = Vector2.Angle(Vector2.left, direction);
        }
        else if (angleToTarget < (angleToCorner - 180) || angleToTarget > (180 - angleToCorner))
        {
            adjacentMagnitude = bounds.height / 2;
            cosineAngle = Vector2.Angle(Vector2.down, direction);
        }

        var hypoteneuseMagnitude = adjacentMagnitude / Mathf.Cos(cosineAngle * Mathf.Deg2Rad);
        return (false, direction.normalized * (hypoteneuseMagnitude - border));
    }

    public static Rect GetCameraSize(Camera camera)
    {
        var yMax = camera.orthographicSize;
        var xMax = yMax * Screen.width / Screen.height;

        var center = new Vector2(camera.transform.position.x, camera.transform.position.y);
        var size = new Vector2(xMax, yMax);

        return new Rect(center - size, size * 2);
    }
}
