//20206208_Nguyễn Hải Phong
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace cuoi_ky
{   

    class Reader {
    private static Thread inputThread;
    private static AutoResetEvent getInput, gotInput;
    private static string input;

    static Reader() {
        getInput = new AutoResetEvent(false);
        gotInput = new AutoResetEvent(false);
        inputThread = new Thread(reader);
        inputThread.IsBackground = true;
        inputThread.Start();
    }

    private static void reader() {
        while (true) {
        getInput.WaitOne();
        input = Console.ReadLine();
        gotInput.Set();
        }
    }

    // omit the parameter to read a line without a timeout
    public static string ReadLine(int timeOutMillisecs = Timeout.Infinite) {
        getInput.Set();
        bool success = gotInput.WaitOne(timeOutMillisecs);
        if (success)
        return input;
        else
        throw new TimeoutException("User did not provide input within the timelimit.");
    }
    }
	public static class Quiz
	{   public static string filepath = @"dethi.txt"; //biến ghi địa chỉ của thư viện đề thi

        public static char GetAnswer(int i) //lấy đáp án của câu hỏi thứ i, dòng thứ 6*i+5 là đáp án của câu hỏi thứ i
		{   
            string[] lines = File.ReadAllLines(filepath);
            return Convert.ToChar(lines[6*i+5]);
        }
		public static void ShowQuiz(int i) //in ra màn hình câu hỏi thứ i
		{   
            string[] lines = File.ReadAllLines(filepath);
            for (int j = 6*i;j<6*i+5;j++)
                Console.WriteLine(lines[j]);
        }

        public static int CountQuiz() //đếm số câu hỏi có trong bộ đề, cứ 6 dòng lại là một câu
		{   
            int linescount = File.ReadAllLines(filepath).Length;
            int count = linescount/6;
            Console.WriteLine("số câu hỏi trong hệ thống: "+count);
            return count;
        }

        public static int[] Randomize(int i,int QSysNum) //hàm trộn câu hỏi
        {
            var rnd = new Random();
            int[] quizList = new int[i];
            quizList = Enumerable.Range(0,QSysNum).OrderBy(x => rnd.Next()).Take(i).ToArray();
            return quizList;
        }

        public static int kiemtradauvao(int QSysNum) //kiểm tra điều kiện nhập số câu
        {   
            Console.Write("nhập số câu muốn làm: ");//nhập số câu muốn làm
            int Qnum;
            if (int.TryParse(Console.ReadLine(),out Qnum))
            {
                if(Qnum>QSysNum)//trường hợp nhập quá số câu hỏi có trong ngân hàng đề
                {
                    Console.WriteLine("hiện chỉ có " + QSysNum + " câu hỏi, hãy nhập số lượng câu hỏi ít hơn");
                    kiemtradauvao(QSysNum);
                }

                if(Qnum<=0){//trường hợp nhập số nhỏ hơn hoặc bằng 0
                    Console.WriteLine("sai định dạng, hãy nhập lại");
                    kiemtradauvao(QSysNum);
                }
                return Qnum;
            }
            else
            {   //trường hợp không phải là định dạng số
                Console.WriteLine("sai định dạng, hãy nhập lại");
                kiemtradauvao(QSysNum);
                return Qnum;
            }
        }

        public static void Maivn(string[] args)
        {   
            Console.OutputEncoding = System.Text.Encoding.UTF8; // in ra được tiếng việt unicode
            int correct = 0;    //biến đếm số câu đúng
            List<int> wrongkey= new List<int>();    // list các câu sai
            List<int> wrongkeynum= new List<int>(); // thứ tự của các câu sai khi đã trộn
            List<char> wrongchoice= new List<char>();  //list câu trả lời của các câu sai 
            int QSysNum=CountQuiz(); //đếm số câu hỏi
            int Qnum = kiemtradauvao(QSysNum); //kiểm tra điều kiện nhập
            int[] quizList = Randomize(Qnum,QSysNum); // trộn các câu hỏi
            int time = Qnum*60*1000; //thời gian làm bài: bằng số câu*60 giây

            //làm bài
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();//bắt đầu đếm thời gian
            try
            {
                for(int j=0;j<Qnum;j++)
                {   
                    long t = stopWatch.ElapsedMilliseconds;
                    int t1= Convert.ToInt32(t); //thời gian đã qua
                    Console.Clear();
                    Console.WriteLine("Câu số " + (j+1) + ":");
                    Console.WriteLine("thời gian còn lại:"+(time-t1)/1000+" giây");
                    ShowQuiz(quizList[j]);
                    Console.WriteLine("\nNhấn X nếu muốn kết thúc bài thi");
                    Console.WriteLine("Nhấn S để bỏ qua câu hỏi này");
                    char choice;
                    char answer = GetAnswer(j);
                    while (char.TryParse(Reader.ReadLine(time-t1),out choice) == false 
                            || choice != 'A' 
                            && choice != 'B' 
                            && choice != 'C'                    //các trường hợp nhập sai định dạng đáp án
                            && choice != 'D'
                            && choice != 'S'
                            && choice != 'X'
                            )
                        Console.WriteLine("\n nhập sai định dạng, chỉ nhập các phương án A B C D hoặc nhập X,S");
                    if(choice == 'X') break; //trường hợp muốn kết thúc bài thi sớm
                    if(choice == 'S') continue; //trường hợp bỏ qua câu hỏi tiếp theo
                    if (choice == answer) //trường hợp trả lời đúng
                    {
                        correct++;
                    }
                    else //trường họp trả lời đúng
                    {   
                        wrongkeynum.Add(j+1);
                        wrongkey.Add(quizList[j]);
                        wrongchoice.Add(choice);
                    }
                }
                Console.Clear();
            }
            //xử lý khi hết thời gian làm bài
            catch (TimeoutException) 
            {
                Console.Clear();
                Console.WriteLine("đã hết giờ\n");
            }
            stopWatch.Stop();

            //ghi ra kết quả
            Console.WriteLine("bạn đã làm đúng: " +correct+"/"+Qnum+" câu");
            Console.WriteLine("Các câu sai:");
            int resultcount=0; //
            foreach (int i in wrongkey)
            {
                Console.WriteLine("Câu số " + wrongkeynum[resultcount] + ":");
                ShowQuiz(i);
                Console.WriteLine("Bạn điền: " + wrongchoice[resultcount]);
                Console.WriteLine("Đáp án: " + GetAnswer(i) +"\n");
                resultcount++;
            }
        }
	}
}
