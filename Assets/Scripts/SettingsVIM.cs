using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsVIM : MonoBehaviour {

    public static SettingsVIM link;

    [Header("��������� ������������")]
    public float limitMinYRotationBatut;
    public float limitMaxYRotationBatut;
    public float limitXRotationBatut;
    public float minVelocityX;
    public float maxVelocityX;
    public float minVelocityY;
    public float maxVelocityY;
    public float sensivityRotationBatut;
    public float shootPowerBatut;

    [Header("��������� ������")]
    public Vector3 cameraOffset;
    public float cameraSpeed;

    [Header("���������� �����")]
    public float timeHolesScale;
    public float timePauseAfterDambaExplosion;
    public float timeWaterRise;
    public float cameraShockTime;
    public float cameraShockSpeed;

    [Header("����������� �������� �� �����")]
    public float timeWallSplash;
    public float timePauseFall;

    [Header("---����������: ��������� ����---")]
    [Tooltip("������������ ���-�� ���������")]
    public int bulletMaxCount;
    [Tooltip("����� ����� ����������")]
    public float bulletFirePauseTime;

    void Awake() {
        link = this;
    }
}
