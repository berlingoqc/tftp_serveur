using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace tp_tftp
{
    public struct sInfoTh
    {
        //structure que j'envoye au construteur des autres threads du serveur (write and read)
        public EndPoint pDistant;
        public string Nom;
        public int OpCode;
    }

    class c69
    {
        //Thread principal du serveur qui ecoute sur le port 69
        private bool _bThread;
        private Thread t;

        public c69()
        {
            t = new Thread(new ThreadStart(th69));
        }
        public void Start()
        {
            t = new Thread(new ThreadStart(th69));
            _bThread = true;
            t.Start();
        }
        public void Stop()
        {
            _bThread = false;
            
        }
        private void th69()
        {
            byte[] binT = new byte[516];
            sInfoTh sInfo = new sInfoTh();
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.ReceiveBufferSize = 50;
            EndPoint pLocal = new IPEndPoint(0,69);
            EndPoint pDistant = new IPEndPoint(0, 0);

            s.Bind(pLocal);

            while (_bThread)
            {
                if (s.Available > 0)
                {
                    int a = s.ReceiveFrom(binT, ref pDistant);
                    sInfo = ValiderTrame(binT);
                    sInfo.pDistant = pDistant;

                    switch (sInfo.OpCode)
                    {
                        case 1:
                            //RRQ case
                            new cR(sInfo);
                            break;
                        case 2:
                            //WRQ case
                            new cW(sInfo);
                            break;
                        case -1:
                            //Erreur case
                            SendErreur(pDistant);
                            break;
                    }
                }
            }
            s.Close();
        }

        private void SendErreur(EndPoint pd)
        {
            Socket so = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            cTrame ad = new cTrame();
            so.SendTo(ad.Erreur("Operation TFTP illegale"), pd);
            so.Close();
 
        }
        private sInfoTh ValiderTrame(byte[] binTexte)
        {
            /*Regarde si la trame recu est un RRQ(0001) ou WRQ(0002)
             *Apres decode le nom du fichier demander 
             */

            //Instancie une structure qui sera remplit et renvoyer selon information de la trame
            sInfoTh sinf = new sInfoTh();
            if (binTexte[0] == 0 && binTexte[1] == 1)
            {
                sinf.OpCode = 1;
 
            }
            else
            {
                if (binTexte[0] == 0 && binTexte[1] == 2)
                {
                    sinf.OpCode = 2;
                }
                else
                {
                    sinf.OpCode = -1;
                    return sinf;
                }
            }
            int i=2;
            while (binTexte[i] != 0x00)
            {
                sinf.Nom += Encoding.ASCII.GetString(binTexte,i,1);
                i++;
            }

            return sinf;
        }
    }
}
