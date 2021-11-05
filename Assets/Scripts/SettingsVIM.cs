using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsVIM : MonoBehaviour {

    public static SettingsVIM link;

    [Header("Настройки толстякомета")]
    public float limitMinYRotationBatut;
    public float limitMaxYRotationBatut;
    public float limitXRotationBatut;
    public float minVelocityX;
    public float maxVelocityX;
    public float minVelocityY;
    public float maxVelocityY;
    public float sensivityRotationBatut;
    public float shootPowerBatut;

    [Header("Настройки камеры")]
    public Vector3 cameraOffset;
    public float cameraSpeed;

    [Header("Разрушение дамбы")]
    public float timeHolesScale;
    public float timePauseAfterDambaExplosion;
    public float timeWaterRise;
    public float cameraShockTime;
    public float cameraShockSpeed;

    [Header("Сплющивание толстяка об стену")]
    public float timeWallSplash;
    public float timePauseFall;

    [Header("---УПРАЗДНЕНО: Настройки игры---")]
    [Tooltip("Максимальное кол-во толстяков")]
    public int bulletMaxCount;
    [Tooltip("Пауза между выстрелами")]
    public float bulletFirePauseTime;

    void Awake() {
        link = this;
    }
}
