﻿using System;
using UnityEngine;

public class Dinossauro : MonoBehaviour
{
    [Header("Pulo")]
    [Range(0f, 180f)]
    [SerializeField]
    private float forcaPulo = 90f;

    [SerializeField]
    private float distanciaNecessariaDoChao = 1.5f;

    private bool _estaNoChao;

    private bool _puloLiberado = true;

    private bool _puloTravado;

    [Header("Queda")]
    [Range(0f, 30f)]
    [SerializeField]
    private float velocidadeQuedaNormal = 5f;

    [Range(0f, 60f)]
    [SerializeField]
    private float velocidadeQuedaAcelerada = 20f;

    // Movimentação

    private bool _andando = true;

    // Posicionamento

    private float _posicaoYInicial;

    [Header("Componentes")]

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Jogo jogo;

    private void Awake()
    {
        _posicaoYInicial = transform.position.y;
    }

    private void Update()
    {
        // Travar pulo
        if (!_puloTravado && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _puloTravado = true;
        }
        else if (_puloTravado
                 && Input.GetKeyDown(KeyCode.UpArrow)
                 && !Input.GetKey(KeyCode.DownArrow))
        {
            _puloTravado = false;
        }

        // Pulo
        if (_estaNoChao
            && _puloLiberado
            && !_puloTravado
            && !Input.GetKey(KeyCode.DownArrow)
            && (Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.Space)))
        {
            _puloLiberado = false;

            rb.velocity = Vector2.zero;
            rb.AddForce(forcaPulo * Vector2.up);

            jogo.pularAudioSource.Play();
        }
        else if (!_estaNoChao && !_puloLiberado)
        {
            _puloLiberado = true;
        }

        // Queda Acelerada
        if (!_estaNoChao
            && Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.gravityScale = velocidadeQuedaAcelerada;
        }
        else if (_estaNoChao)
        {
            rb.gravityScale = velocidadeQuedaNormal;
        }

        // Atualizar animação
        anim.SetBool("Andando", _andando);

        var abaixado = _estaNoChao && Input.GetKey(KeyCode.DownArrow);
        anim.SetBool("Abaixado", abaixado);

        // Clamp Posição Y
        var posicao = transform.position;
        if (posicao.y < _posicaoYInicial)
        {
            transform.position = new Vector3(
                posicao.x,
                _posicaoYInicial,
                posicao.z
            );
        }
    }

    private void FixedUpdate()
    {
        _estaNoChao = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            distanciaNecessariaDoChao,
            1 << LayerMask.NameToLayer("Chão")
        );
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstáculo"))
        {
            anim.SetBool("Morto", true);

            jogo.FimDeJogo();
        }
    }
}
