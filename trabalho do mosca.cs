//Exercicio do Mosca 
using System;
using TESTE_DAPL_email;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic; 
using System.IO; 
using System.Linq; 
using System.Net.Mail; 
using System.Net; 

namespace TESTE_DAPL_email
{
    internal class Usuario
    {
        
        public string Email;
        public string Senha;
        public string SenhaHash;
        public string EmailHash;

        
        public string getEmail()
        {
            return Email;
        }
        public void setEmail(string email)
        {
            this.Email = email;
        }
        public string getSenha()
        {
            return Senha;
        }
        public void setSenha(string senha)
        {
            this.Senha = senha;
        }
        public string getEmailHash()
        {
            return EmailHash;
        }
        private void setEmailHash(string email)
        {
            this.EmailHash = email;
        }
        public string getSenhaHash()
        {
            return SenhaHash;
        }
        private void setSenhaHash(string senha)
        {
            this.SenhaHash = senha;
        }

        
        public Usuario(string email, string senha)
        {
            this.Email = email;
            this.Senha = senha;
            this.SenhaHash = HashSenha(senha);
            this.EmailHash = HashEmail(email);
        }

        public string HashEmail(string email)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(email));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string HashSenha(string senha)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(senha));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
    
    internal class Registro
    {

        public List<Usuario> usuariosCadastrados = new List<Usuario>();

        static string emailCadastro;
        static string senhaCadastro;
        static string emailLogin;
        static string senhaLogin;
        static string emailAdicionado;
        static string senhaAdicionada;
        static string emailRemover;
        static string email;

        public void Cadastro()
        {
            Console.Write("Entre com seu email: ");
            emailCadastro = Console.ReadLine();
            senhaCadastro = GerarSenhaAleatoria();
            EnviarEmail(emailCadastro, senhaCadastro);
            Console.WriteLine($"Uma senha aleatória foi enviada para {emailCadastro}");
            Console.WriteLine();

            Usuario novoUsuario = new Usuario(emailCadastro, senhaCadastro);
            usuariosCadastrados.Add(novoUsuario);


            Console.WriteLine("Sua conta foi criada com sucesso!");
        }

        public void Login()
        {
            Console.WriteLine("\n--- Login --- ");
            Console.Write("Email: ");
            emailLogin = Console.ReadLine();
            Console.Write("Senha: ");
            senhaLogin = Console.ReadLine();

            if (emailLogin == emailCadastro && senhaLogin == senhaCadastro)
            {
                Console.WriteLine("Login bem-sucedido!");

                Usuario novoUsuario = new Usuario(emailLogin, senhaLogin);
                usuariosCadastrados.Add(novoUsuario);
                using (StreamWriter sw = File.AppendText("arq01.txt"))
                {
                    sw.WriteLine(novoUsuario.EmailHash + ";" + novoUsuario.SenhaHash);
                }

                SegundoMenu();
            }
            else
            {
                Console.WriteLine("Email ou senha incorretos. Tente novamente.");
            }
        }

        public void ListaLogins()
        {
            Console.WriteLine("\n--- Lista de logins ---");
            foreach (Usuario usuario in usuariosCadastrados)
            {
                Console.WriteLine($"Email: {usuario.Email} - Criptografado: {usuario.EmailHash}");
            }
        }

        public void AdicionarLogin()
        {
            Console.WriteLine("Digite o email:");
            emailAdicionado = Console.ReadLine();
            Console.WriteLine("Digite a senha:");
            senhaAdicionada = Console.ReadLine();

            Usuario novoUsuario = new Usuario(emailAdicionado, senhaAdicionada);
            usuariosCadastrados.Add(novoUsuario);

            using (StreamWriter sw = File.AppendText("arq01.txt"))
            {
                sw.WriteLine(novoUsuario.EmailHash + ";" + novoUsuario.SenhaHash);
            }

            Console.WriteLine("Login adicionado com sucesso!");
        }


        public void RemoverLogin()
        {
            Console.WriteLine("Digite o email do login a ser removido:");
            emailRemover = Console.ReadLine();

            Usuario usuarioRemover = usuariosCadastrados.FirstOrDefault(u => u.Email == emailRemover);
            if (usuarioRemover != null)
            {
                usuariosCadastrados.Remove(usuarioRemover);
                Console.WriteLine("Login removido com sucesso!");
            }
            else
            {
                Console.WriteLine("Email não encontrado.");
            }
        }

        static void EnviarEmail(string destinatario, string senha)
        {
            string remetente = "ritadecassiafariacosta@outlook.com";
            string assunto = "Senha Aleatória";
            string corpo = $"Sua senha aleatória é: {senha}";

            MailMessage mensagem = new MailMessage(remetente, destinatario, assunto, corpo);

            SmtpClient cliente = new SmtpClient("smtp.office365.com"); 
            cliente.Port = 587; 
            cliente.Credentials = new NetworkCredential(remetente, "Geraldo1357");
            cliente.EnableSsl = true;

            try
            {
                cliente.Send(mensagem);
                Console.WriteLine("Email enviado com sucesso!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar email: " + ex.Message);
            }
        }


        public void EsqueciSenha()
        {
            Console.WriteLine("=== Esqueci minha senha === ");
            Console.Write("Entre com seu email: ");
            email = Console.ReadLine();

            Usuario usuario = usuariosCadastrados.FirstOrDefault(u => u.Email == email);
            if (usuario != null)
            {
                string senhaNova = GerarSenhaAleatoria();
                EnviarEmail(email, senhaNova);
                Console.WriteLine($"Uma nova senha aleatória foi enviada para {email}");
                Console.WriteLine();

                Usuario UsuarioEsqueciSenha = new Usuario(email, senhaNova);
                usuariosCadastrados.Add(UsuarioEsqueciSenha);
            }
            else
            {
                Console.WriteLine("Email não encontrado.");
            }

        }


        static string GerarSenhaAleatoria()
        {
            Random random = new Random();
            int senhaNumerica = random.Next(100000, 1000000);
            return senhaNumerica.ToString();
        }

        public void SegundoMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- MENU 2 ---");
                Console.WriteLine("1-Lista de logins");
                Console.WriteLine("2-Adicionar login");
                Console.WriteLine("3-Remover login");
                Console.WriteLine("4-Sair do programa");
                Console.WriteLine("  Escolha uma opção:");
                int opcao = int.Parse(Console.ReadLine());

                switch (opcao)
                {
                    case 1:
                        ListaLogins();
                        break;
                    case 2:
                        AdicionarLogin();
                        break;
                    case 3:
                        RemoverLogin();
                        break;
                    case 4:
                        Console.WriteLine("Saindo do programa");
                        return;
                    default:
                        Console.WriteLine("Opção inválida. Tente novamente.");
                        break;
                }
            }
        }
    }
}

class Programa
{
    static void Main()
    {
        Registro registro = new Registro();

        while (true)
        {
            Console.WriteLine("\n--- MENU 1 ---");
            Console.WriteLine("1-Cadastrar conta");
            Console.WriteLine("2-Fazer login");
            Console.WriteLine("3-Esqueci minha senha");
            Console.WriteLine("4-Sair do programa");
            Console.WriteLine("  Escolha uma opção:");

            int opcao = int.Parse(Console.ReadLine());

            switch (opcao)
            {
                case 1:
                    registro.Cadastro();
                    break;

                case 2:
                    registro.Login();
                    break;

                case 3:
                    registro.EsqueciSenha();
                    break;

                case 4:
                    Console.WriteLine("Saindo do programa");
                    return;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
        }
    }
}