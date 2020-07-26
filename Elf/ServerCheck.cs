using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Elf
{
    public class ServerCheck
    {
        // Could be IPv4 or IPv6
        public IPAddress hostAddress { get; }
        public short hostPort { get; }

        // Expected host name in certificate
        public String hostName { get; }

        // Whether to use SNI
        public bool useSNI { get; }

        // Allowed protocols
        public SslProtocols enabledProtocols { get; }

        public ServerCheck(IPAddress address, short port, String hostName, bool useSNI, SslProtocols protocols = SslProtocols.Tls)
        {
                this.hostAddress = address;
                this.hostPort = port;
                this.hostName = hostName;
                this.useSNI = useSNI;
        }

        public void run()
        {
            TcpClient tcpClient = new TcpClient(hostAddress.AddressFamily);
            tcpClient.Connect(hostAddress, hostPort);

            SslStream sslStream = new SslStream(tcpClient.GetStream(),false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                new LocalCertificateSelectionCallback(SelectClientCertificate),
                EncryptionPolicy.AllowNoEncryption);

            //X509Certificate clientCertificate = new X509Certificate(new byte[0], "password");
            X509CertificateCollection clientCertificates = new X509CertificateCollection(new X509Certificate [] { } );

            try
            {
                sslStream.AuthenticateAsClient(
                    useSNI ? hostName : "",
                    clientCertificates,
                    enabledProtocols,
                    true);
            }
            catch (AuthenticationException e)
            {
                // Report whatever the problem is
                Console.WriteLine(e);
            }

            Console.WriteLine("Connection established and authenticated");
            Console.WriteLine($"Protocol: {sslStream.SslProtocol}");
        }

        private X509Certificate SelectClientCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            Console.WriteLine(
                "Client certificate requested:\n" +
                $"Target host: {targetHost}",
                $"Issuers: {acceptableIssuers}");
            return null;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            Console.WriteLine("Validating server certificate:\n" +
                $"Cert  : {certificate}\n" +
                $"Chain : {chain}\n" +
                $"Errors: {sslPolicyErrors}");
            return true;
        }
    }
}
