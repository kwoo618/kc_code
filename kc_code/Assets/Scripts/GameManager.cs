using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic; // List 사용을 위해 필수

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static bool isGameStarted = false;

    public bool isGameOver = false;

    [Header("--- 플레이어 설정 ---")]
    public GameObject player;

    [Header("--- 패널 연결 ---")]
    public GameObject startPanel;
    public GameObject successPanel;
    public GameObject failMoneyPanel;
    public GameObject failStressPanel;

    public GameObject bankPanel;
    public GameObject storePanel;
    public GameObject academyPanel;
    public GameObject reportPanel;
    public GameObject nextPanel;
    public GameObject pausePanel; // [추가] 일시정지 메뉴 패널

    [Header("--- 알림용 패널 ---")]
    public GameObject alertPanel;
    public GameObject insufficientFundsPanel;
    public GameObject duplicateActionPanel;
    public GameObject highStressPanel;
    public GameObject lowStressPanel;
    public GameObject accidentPanel;

    [Header("--- 효과 및 연출 ---")]
    public Image fadeImage;
    public TextMeshProUGUI tipText;

    [Header("--- 상단 HUD ---")]
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI jobLevelText;

    [Header("--- 명세서 텍스트 ---")]
    public TextMeshProUGUI txtReportSalary;
    public TextMeshProUGUI txtReportPension;
    public TextMeshProUGUI txtReportHealth;
    public TextMeshProUGUI txtReportTax;
    public TextMeshProUGUI txtReportLoan;
    public TextMeshProUGUI txtReportNetPay;
    public TextMeshProUGUI txtReportLiving;
    public TextMeshProUGUI txtReportSavings;
    public TextMeshProUGUI txtReportS_Interest;
    public TextMeshProUGUI txtReportFinal;

    [Header("--- 결과 및 알림 텍스트 ---")]
    public TextMeshProUGUI txtResultScore;
    public TextMeshProUGUI txtAlertMsg;

    [Header("--- 버튼 ---")]
    public Button savingsJoinBtn;
    public TextMeshProUGUI savingsBtnText;

    [Header("--- 퀴즈 시스템 ---")]
    public GameObject quizTryPanel;       // 퀴즈 풀지 말지 여부 확인 패널
    public GameObject quizPanel;          // 퀴즈 팝업 패널
    public TextMeshProUGUI quizQuestion;  // 문제 텍스트
    public Button[] quizButtons;          // 보기 버튼 3개
    public TextMeshProUGUI[] quizBtnTexts;// 보기 버튼의 텍스트 3개
    public GameObject quizCorrectPanel;   // 정답일 때 띄울 패널
    public GameObject quizWrongPanel;     // 틀렸을 때 띄울 패널
    public TextMeshProUGUI txtWrongAnswer; // "정답은 2번입니다!" 라고 써줄 텍스트

    [Header("--- 게임 데이터 및 밸런스 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;
    public int baseSalary = 2000000;

    // [밸런스] 수치 조정 영역
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 1000000;

    // [추가] 밸런스 변수
    public int maxJobLevel = 10;
    public int salaryIncreasePerLevel = 200000;

    // [추가] 행동 비용
    public int studyCost = 150000;
    public int promotionCost = 1000000;
    public int convenienceCost = 100000;

    private bool isSavingsJoined = false;
    private bool hasSelfDevThisMonth = false;
    private bool hasQuizThisMonth = false; // [추가] 이번 달 퀴즈 수행 여부

    // [추가] 중복 방지를 위한 문제 번호 리스트
    private List<int> availableQuizIndices = new List<int>();

    // [수정] 게임 내 콘텐츠와 직접 관련된 팁
    private string[] financialFacts = new string[]
    {
        "[Tip!] 적금에 가입하면 만기 시 '원금 + 이자'를\n받을 수 있어 목돈 마련에 유리합니다.",
        "[Tip!] '복리'란 이자에 또 이자가 붙는 효과입니다.\n게임에서도 저축액이 늘어날수록 이자가 커집니다!",
        "[Tip!] 스트레스가 100%가 되면 건강을 잃고 게임이 종료됩니다.\n적절한 휴식도 투자입니다.",
        "[Tip!] 갑작스러운 교통사고에 대비해 항상\n'비상금(현금)'을 남겨둬야 파산을 막을 수 있습니다.",
        "[Tip!] 대출을 받으면 매달 이자가 지출됩니다.\n감당할 수 있는 능력 안에서만 빌리세요.",
        "[Tip!] 월급 명세서의 '실수령액'은 세금과 보험료를 뗀,\n실제로 내가 쓸 수 있는 돈입니다.",
        "[Tip!] 현금이 바닥나면 파산하게 됩니다.\n수입보다 지출이 많지 않도록 관리하세요."
    };

    [System.Serializable]
    public struct QuizData
    {
        public string question;
        public string[] answers;
        public int correctAnswer; // 0, 1, 2 중 하나
    }

    // [수정] 40개의 팩트 기반 퀴즈 데이터
    private QuizData[] quizzes = new QuizData[]
    {
        // 1. 경제 용어 정의
        new QuizData { question = "'복리'의 의미로 올바른 것은?", answers = new string[] { "원금에만 이자가 붙음", "이자에 이자가 붙음", "대출 이자가 줄어듦" }, correctAnswer = 1 },
        new QuizData { question = "예금자보호법으로 보호받는 한도는?", answers = new string[] { "인당 3천만원", "인당 5천만원", "인당 1억원" }, correctAnswer = 1 },
        new QuizData { question = "주식에서 기업이 이익을 주주에게 나눠주는 것은?", answers = new string[] { "배당", "이자", "상환" }, correctAnswer = 0 },
        new QuizData { question = "물가가 지속적으로 오르는 현상은?", answers = new string[] { "디플레이션", "스태그플레이션", "인플레이션" }, correctAnswer = 2 },
        new QuizData { question = "신용점수가 낮아지면 발생하는 불이익은?", answers = new string[] { "대출 금리 상승", "취업 즉시 제한", "은행 이용 불가" }, correctAnswer = 0 },
     
        // 2. 격언 및 상식
        new QuizData { question = "분산 투자의 중요성을 강조한 격언은?", answers = new string[] { "계란을 한 바구니에 담지 마라", "티끌 모아 태산", "소 잃고 외양간 고친다" }, correctAnswer = 0 },
        new QuizData { question = "소득에서 세금 등을 뺀 실제 쓸 수 있는 돈은?", answers = new string[] { "총급여", "실수령액", "기본급" }, correctAnswer = 1 },
     
        // 3. 상품의 특성 비교
        new QuizData { question = "은행에 돈을 맡기는 가장 안전한 방법은?", answers = new string[] { "주식", "예금", "가상화폐" }, correctAnswer = 1 },
        new QuizData { question = "국가에 납부하는 필수 비용은?", answers = new string[] { "기부금", "세금", "배당금" }, correctAnswer = 1 },
        new QuizData { question = "수입보다 지출이 많을 때 발생하는 상태는?", answers = new string[] { "흑자", "적자", "무역" }, correctAnswer = 1 },
        new QuizData { question = "돈의 가치가 떨어지고 물가가 오르는 이유는?", answers = new string[] { "화폐 공급 증가", "화폐 공급 감소", "수입 증가" }, correctAnswer = 0 },
        new QuizData { question = "중앙은행이 결정하는 기본 금리는?", answers = new string[] { "시장금리", "우대금리", "기준금리" }, correctAnswer = 2 },
        new QuizData { question = "돈을 빌려준 대가로 받는 돈은?", answers = new string[] { "이자", "원금", "할부금" }, correctAnswer = 0 },
     
        // 4. 행동에 따른 결과
        new QuizData { question = "나라 간의 돈을 바꾸는 비율은?", answers = new string[] { "금리", "환율", "주가" }, correctAnswer = 1 },
        new QuizData { question = "개인의 경제적 신용도를 숫자로 나타낸 것은?", answers = new string[] { "신용점수", "시험점수", "통장잔고" }, correctAnswer = 0 },
        new QuizData { question = "원금 손실 가능성이 있는 금융 상품은?", answers = new string[] { "정기예금", "펀드/주식", "청약저축" }, correctAnswer = 1 },
        new QuizData { question = "월급에서 미리 떼어가는 세금 제도는?", answers = new string[] { "원천징수", "연말정산", "부가세" }, correctAnswer = 0 },
        new QuizData { question = "한 나라의 경제 규모를 나타내는 지표는?", answers = new string[] { "GDP", "CPI", "KOSPI" }, correctAnswer = 0 },
        new QuizData { question = "빚을 갚지 못해 법적으로 선언하는 상태는?", answers = new string[] { "파산", "정지", "해지" }, correctAnswer = 0 },

        // 5. 세금 및 제도
        new QuizData { question = "1년 동안 낸 세금을 정산하여 더 낸 돈을 돌려받거나 더 내는 절차는?", answers = new string[] { "부가가치세 신고", "연말정산", "분리과세" }, correctAnswer = 1 },
        new QuizData { question = "현금 결제 시 발급받아 연말정산 소득공제 혜택을 받는 영수증은?", answers = new string[] { "간이영수증", "현금영수증", "세금계산서" }, correctAnswer = 1 },
        new QuizData { question = "매달 월세를 내지 않고 보증금을 맡겼다가 돌려받는 임대차 제도는?", answers = new string[] { "월세", "매매", "전세" }, correctAnswer = 2 },
        new QuizData { question = "새 아파트 분양(청약) 자격을 얻기 위해 가입하는 필수 통장은?", answers = new string[] { "주택청약종합저축", "정기예금", "마이너스통장" }, correctAnswer = 0 },

        // 6. 은행 및 대출 상식
        new QuizData { question = "대출 만기까지 금리가 변하지 않고 일정하게 유지되는 방식은?", answers = new string[] { "변동금리", "고정금리", "가산금리" }, correctAnswer = 1 },
        new QuizData { question = "담보 없이 개인의 신용(소득, 직업 등)만 보고 빌려주는 대출은?", answers = new string[] { "담보대출", "신용대출", "전세자금대출" }, correctAnswer = 1 },
        new QuizData { question = "소득 대비 전체 빚의 원리금 상환액 비율을 따지는 규제는?", answers = new string[] { "DSR", "LTV", "BIS" }, correctAnswer = 0 },
        new QuizData { question = "카드 대금의 일부만 결제하고 나머지는 이월하여 갚는 서비스는?", answers = new string[] { "할부", "리볼빙", "포인트 결제" }, correctAnswer = 1 },

        // 7. 투자 및 경제 용어
        new QuizData { question = "주식처럼 시장에서 자유롭게 사고팔 수 있는 펀드 상품은?", answers = new string[] { "예금", "ETF", "적금" }, correctAnswer = 1 },
        new QuizData { question = "재무 구조가 튼튼하고 수익성이 좋은 대형 우량주를 뜻하는 말은?", answers = new string[] { "블루칩", "옐로칩", "레드칩" }, correctAnswer = 0 },
        new QuizData { question = "주식 시장에서 주가가 계속 하락하는 약세장을 뜻하는 말은?", answers = new string[] { "불 마켓", "베어 마켓", "프리 마켓" }, correctAnswer = 1 },
        new QuizData { question = "어떤 선택으로 인해 포기해야 하는 다른 선택지의 가치는?", answers = new string[] { "매몰비용", "기회비용", "유지비용" }, correctAnswer = 1 },
        new QuizData { question = "물가가 지속적으로 하락하고 경제 활동이 침체되는 현상은?", answers = new string[] { "디플레이션", "인플레이션", "스태그플레이션" }, correctAnswer = 0 },
        new QuizData { question = "국제 거래의 결제 수단으로 통용되는 핵심 통화(예: 달러)는?", answers = new string[] { "가상화폐", "기축통화", "지역화폐" }, correctAnswer = 1 },

        // 8. 금융 편의 및 보안
        new QuizData { question = "하루만 맡겨도 이자를 주며 입출금이 자유로운 증권사 통장은?", answers = new string[] { "CMA", "적금", "청약통장" }, correctAnswer = 0 },
        new QuizData { question = "하나의 앱에서 모든 은행 계좌를 조회하고 이체할 수 있는 서비스는?", answers = new string[] { "오픈뱅킹", "텔레뱅킹", "프라이빗뱅킹" }, correctAnswer = 0 },
        new QuizData { question = "매번 새로운 비밀번호가 생성되는 일회용 비밀번호 생성기는?", answers = new string[] { "보안카드", "OTP", "공인인증서" }, correctAnswer = 1 },
        new QuizData { question = "문자(SMS) 속 인터넷 주소를 눌러 악성코드를 설치하는 사기는?", answers = new string[] { "보이스피싱", "스미싱", "파밍" }, correctAnswer = 1 },
    };

    void Awake() { instance = this; }

    void Start()
    {
        CloseAllPanels();
        UpdateUI();

        if (tipText) tipText.gameObject.SetActive(false);

        if (!isGameStarted)
        {
            Time.timeScale = 0;
            if (startPanel) startPanel.SetActive(true);

            // [추가] 퀴즈 풀 초기화
            ResetQuizPool();

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayBGM(SoundManager.instance.titleBgm);
            }
        }
        else
        {
            Time.timeScale = 1;
            if (startPanel) startPanel.SetActive(false);

            if (SoundManager.instance) SoundManager.instance.PlayHomeBGM();
        }
    }

    // [추가] ESC 키 입력을 감지하는 Update 함수
    void Update()
    {
        // 게임이 시작되지 않았거나, 이미 게임오버 상태라면 ESC 작동 안 함
        if (!isGameStarted || isGameOver) return;

        // ESC 키 입력 확인
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // (선택사항) 만약 중요한 팝업(퀴즈, 사고, 결과창 등)이 켜져있다면 일시정지를 막을 수 있음
            // if (quizPanel.activeSelf || reportPanel.activeSelf) return;

            OnTogglePause();
        }
    }

    // [추가] 퀴즈 풀(Pool) 초기화 함수
    void ResetQuizPool()
    {
        availableQuizIndices.Clear();
        for (int i = 0; i < quizzes.Length; i++)
        {
            availableQuizIndices.Add(i);
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
        isGameOver = false;
        Time.timeScale = 1;
        if (startPanel) startPanel.SetActive(false);
        UpdateUI();

        if (SoundManager.instance)
            SoundManager.instance.PlayHomeBGM();
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void GameRestart()
    {
        isGameStarted = false;
        ResetGameData();
        SceneManager.LoadScene("CompanyMain");
    }

    public void GameRetry()
    {
        isGameStarted = true;
        ResetGameData();
        SceneManager.LoadScene("CompanyMain");
    }

    void ResetGameData()
    {
        Time.timeScale = 1;
        isGameOver = false;
        currentMonth = 1;
        cash = 300000;
        stress = 0;
        savings = 0;
        loan = 0;
        jobLevel = 1;
        baseSalary = 2000000;
        isSavingsJoined = false;
        hasSelfDevThisMonth = false;
        hasQuizThisMonth = false;

        // [추가] 재시작 시 퀴즈 풀도 리셋
        ResetQuizPool();
    }

    public void OnClickNextMonth()
    {
        if (currentMonth > 10 || isGameOver) return;

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.nextMonthSfx);

        if (stress + 40 >= 100)
        {
            ShowHighStressPanel();
            return;
        }

        if (cash < 0)
        {
            EndGame("파산");
            return;
        }

        CloseAllPanels();
        CalculateAndShowReport();

        if (reportPanel) reportPanel.SetActive(true);
        Time.timeScale = 0;
    }

    void CalculateAndShowReport()
    {
        int currentSalary = baseSalary;

        // 반올림 적용 (계산 오차 해결)
        int pension = Mathf.RoundToInt(currentSalary * 0.045f);
        int health = Mathf.RoundToInt(currentSalary * 0.035f);
        int tax = Mathf.RoundToInt(currentSalary * 0.03f);
        int loanInterest = (loan > 0) ? Mathf.RoundToInt(loan * 0.02f) : 0;

        int totalDeduction = pension + health + tax + loanInterest;
        int netPay = currentSalary - totalDeduction; // 순수 실수령액

        // 적금 로직 (실수령액이 아니라 현재 현금 + 들어올 돈으로 계산)
        bool isSavingsSuccess = false;
        int savingsAmount = 0;

        if (isSavingsJoined)
        {
            if (cash + netPay >= MONTHLY_SAVINGS_AMOUNT)
            {
                savingsAmount = MONTHLY_SAVINGS_AMOUNT;
                savings += savingsAmount;
                isSavingsSuccess = true;
            }
        }

        int interest = Mathf.RoundToInt(savings * 0.005f);
        savings += interest;

        // 최종 현금 변동 (실수령액 - 생활비 - 저축액)
        int actualChange = netPay - LIVING_COST - savingsAmount;
        cash += actualChange;

        stress += 40;

        UpdateUI();

        // UI 표시
        if (txtReportSalary) txtReportSalary.text = $"{currentSalary:N0}";
        if (txtReportPension) txtReportPension.text = $"-{pension:N0}";
        if (txtReportHealth) txtReportHealth.text = $"-{health:N0}";
        if (txtReportTax) txtReportTax.text = $"-{tax:N0}";
        if (txtReportLoan) txtReportLoan.text = $"-{loanInterest:N0}";
        if (txtReportNetPay) txtReportNetPay.text = $"{netPay:N0}";
        if (txtReportLiving) txtReportLiving.text = $"-{LIVING_COST:N0}";

        if (txtReportSavings)
        {
            if (!isSavingsJoined) txtReportSavings.text = "미가입";
            else txtReportSavings.text = isSavingsSuccess ? $"-{savingsAmount:N0}" : "잔액 부족";
        }

        if (txtReportS_Interest) txtReportS_Interest.text = $"+{interest:N0}";
        if (txtReportFinal) txtReportFinal.text = $"{actualChange:N0}";
    }

    public void OnConfirmReport()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (reportPanel) reportPanel.SetActive(false);
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        if (tipText != null && financialFacts.Length > 0)
        {
            tipText.gameObject.SetActive(true);
            int randIdx = Random.Range(0, financialFacts.Length);
            tipText.text = financialFacts[randIdx];
        }

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        yield return new WaitForSecondsRealtime(1.0f);

        currentMonth++;
        hasSelfDevThisMonth = false;
        hasQuizThisMonth = false;

        if (currentMonth > 10)
        {
            if (tipText) tipText.gameObject.SetActive(false);
            EndGame("완료");
            yield break;
        }

        if (stress >= 100)
        {
            if (tipText) tipText.gameObject.SetActive(false);
            EndGame("스트레스");
            yield break;
        }

        UpdateUI();

        if (fadeImage != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }

        if (tipText) tipText.gameObject.SetActive(false);

        Time.timeScale = 1;
    }

    void ShowInsufficientFundsPanel() { CloseAllPanels(); if (insufficientFundsPanel) insufficientFundsPanel.SetActive(true); }
    void ShowHighStressPanel() { CloseAllPanels(); if (highStressPanel) highStressPanel.SetActive(true); }
    void ShowLowStressPanel() { CloseAllPanels(); if (lowStressPanel) lowStressPanel.SetActive(true); }
    void ShowDuplicateActionPanel() { CloseAllPanels(); if (duplicateActionPanel) duplicateActionPanel.SetActive(true); }

    public void CloseAlert()
    {
        if (alertPanel) alertPanel.SetActive(false);
        if (insufficientFundsPanel) insufficientFundsPanel.SetActive(false);
        if (highStressPanel) highStressPanel.SetActive(false);
        if (lowStressPanel) lowStressPanel.SetActive(false);
        if (duplicateActionPanel) duplicateActionPanel.SetActive(false);
        if (accidentPanel) accidentPanel.SetActive(false);

        if (quizTryPanel) quizTryPanel.SetActive(false);
        if (quizPanel) quizPanel.SetActive(false);
        if (quizCorrectPanel) quizCorrectPanel.SetActive(true);
        if (quizWrongPanel) quizWrongPanel.SetActive(true);

        if (quizCorrectPanel) quizCorrectPanel.SetActive(false);
        if (quizWrongPanel) quizWrongPanel.SetActive(false);

        // 중요 패널들이 다 꺼졌다면 시간 재개
        if (!reportPanel.activeSelf && !accidentPanel.activeSelf && !quizPanel.activeSelf && !quizTryPanel.activeSelf && (pausePanel == null || !pausePanel.activeSelf))
        {
            Time.timeScale = 1;
        }
    }

    public void ActionPromotion()
    {
        if (isGameOver) return;
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (hasSelfDevThisMonth) { ShowDuplicateActionPanel(); return; }

        if (jobLevel >= maxJobLevel)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "이미 최고 직무 레벨입니다.";
            }
            return;
        }

        if (stress + 40 >= 100) { ShowHighStressPanel(); return; }
        if (cash < promotionCost) { ShowInsufficientFundsPanel(); return; }

        cash -= promotionCost;

        int levelGain = 3;
        if (jobLevel + levelGain > maxJobLevel) levelGain = maxJobLevel - jobLevel;

        jobLevel += levelGain;
        baseSalary += (salaryIncreasePerLevel * levelGain);
        stress += 40;

        hasSelfDevThisMonth = true;
        CloseAllPanels(); UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = $"실무 참여 완료!\n직무 레벨 +{levelGain}\n월급이 대폭 인상되었습니다.";
        }
    }

    public void ActionStudy()
    {
        if (isGameOver) return;
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (hasSelfDevThisMonth) { ShowDuplicateActionPanel(); return; }

        if (jobLevel >= maxJobLevel)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "이미 최고 직무 레벨입니다.";
            }
            return;
        }

        if (stress + 10 >= 100) { ShowHighStressPanel(); return; }
        if (cash < studyCost) { ShowInsufficientFundsPanel(); return; }

        cash -= studyCost;
        jobLevel += 1;
        baseSalary += salaryIncreasePerLevel;
        stress += 10;

        hasSelfDevThisMonth = true;
        CloseAllPanels(); UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "직무 강의 수강 완료!\n직무 레벨 +1";
        }
    }

    public void ActionBorrow()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan >= 2000000) return; loan += 500000; cash += 500000; UpdateUI();
    }

    public void ActionRepay()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan <= 0) return; if (cash < 500000) { ShowInsufficientFundsPanel(); return; }
        loan -= 500000; cash -= 500000; UpdateUI();
    }

    public void ActionJoinSavings()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (isGameOver || isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "정기적금 가입 완료!\n매달 500,000원이 저축됩니다.\n(게임 종료시 이자포함 지급)";
        }
    }

    public void ActionBuy()
    {
        if (isGameOver) return;
        if (stress <= 0) { ShowLowStressPanel(); return; }

        if (cash < convenienceCost) { ShowInsufficientFundsPanel(); return; }

        cash -= convenienceCost;
        stress -= 30;
        if (stress < 0) stress = 0;

        CloseAllPanels();
        UpdateUI();

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.eatSfx);

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "이용 완료!\n스트레스 -30";
        }
    }

    public void ActionRest()
    {
        if (isGameOver) return;
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (stress <= 0) { ShowLowStressPanel(); return; }
        stress -= 20; if (stress < 0) stress = 0; UpdateUI();
    }

    public void OnCarAccident()
    {
        if (isGameOver || (accidentPanel != null && accidentPanel.activeSelf)) return;

        int penalty = 200000;

        cash -= penalty;
        stress += 20; if (stress > 100) stress = 100;

        UpdateUI();

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.crashSfx);

        if (accidentPanel)
        {
            accidentPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OnConfirmAccident()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (accidentPanel) accidentPanel.SetActive(false);

        if (cash < 0)
        {
            EndGame("파산");
            return;
        }

        if (stress >= 100)
        {
            EndGame("스트레스");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(CarAccidentSequence());
    }

    IEnumerator CarAccidentSequence()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
        }

        if (player != null) player.transform.position = new Vector3(95.4f, -65.5f, 0);

        currentMonth++;
        hasSelfDevThisMonth = false;

        if (currentMonth > 10) { EndGame("완료"); yield break; }

        UpdateUI();
        yield return new WaitForSecondsRealtime(0.5f);

        if (fadeImage != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void ShowQuizConfirm()
    {
        if (quizTryPanel == null || isGameOver) return;

        if (hasQuizThisMonth)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "퀴즈는 한 달에 한 번만 참여할 수 있습니다.";
            }
            return;
        }

        Time.timeScale = 0;
        quizTryPanel.SetActive(true);
    }

    public void StartQuiz()
    {
        if (quizTryPanel) quizTryPanel.SetActive(false);
        if (quizPanel == null) return;

        quizPanel.SetActive(true);

        if (availableQuizIndices.Count == 0)
        {
            ResetQuizPool();
        }

        int randomListIndex = Random.Range(0, availableQuizIndices.Count);
        int realQuizIndex = availableQuizIndices[randomListIndex];
        availableQuizIndices.RemoveAt(randomListIndex);

        QuizData selectedQuiz = quizzes[realQuizIndex];
        quizQuestion.text = selectedQuiz.question;

        int displayCorrectNum = selectedQuiz.correctAnswer + 1;

        for (int i = 0; i < 3; i++)
        {
            int index = i;
            quizBtnTexts[i].text = selectedQuiz.answers[i];
            quizButtons[i].onClick.RemoveAllListeners();
            quizButtons[i].onClick.AddListener(() => OnClickAnswer(index == selectedQuiz.correctAnswer, displayCorrectNum));
        }
    }

    void OnClickAnswer(bool isCorrect, int correctNum)
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        hasQuizThisMonth = true;
        quizPanel.SetActive(false);

        if (isCorrect)
        {
            cash += 100000;
            UpdateUI();
            if (quizCorrectPanel) quizCorrectPanel.SetActive(true);
        }
        else
        {
            if (quizWrongPanel)
            {
                quizWrongPanel.SetActive(true);
                if (txtWrongAnswer != null)
                    txtWrongAnswer.text = $"오답입니다!\n정답은 <color=yellow>{correctNum}번</color>입니다.";
            }
        }
    }

    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (nextPanel) nextPanel.SetActive(false);
        if (alertPanel) alertPanel.SetActive(false);
        if (insufficientFundsPanel) insufficientFundsPanel.SetActive(false);
        if (duplicateActionPanel) duplicateActionPanel.SetActive(false);
        if (highStressPanel) highStressPanel.SetActive(false);
        if (lowStressPanel) lowStressPanel.SetActive(false);
        if (accidentPanel) accidentPanel.SetActive(false);
        if (quizTryPanel) quizTryPanel.SetActive(false);
        if (quizPanel) quizPanel.SetActive(false);
        if (quizCorrectPanel) quizCorrectPanel.SetActive(false);
        if (quizWrongPanel) quizWrongPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false); // [추가] 일시정지 패널도 닫기
    }

    // [추가] 일시정지 패널을 켜고 끄는 함수 (토글)
    // ESC 키를 누르거나, '계속하기' 버튼을 누를 때 호출됩니다.
    public void OnTogglePause()
    {
        if (pausePanel == null) return;

        bool isActive = pausePanel.activeSelf;

        // 켜져있으면 -> 끄고 시간 흐르게
        if (isActive)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        // 꺼져있으면 -> 켜고 시간 멈춤
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
    }

    // [추가] 메인 메뉴로 돌아가기 (시간 정상화 후 재시작)
    public void OnClickMainMenu()
    {
        Time.timeScale = 1;
        GameRestart();
    }

    // [추가] 다시 시작하기 (시간 정상화 후 재시도)
    public void OnClickRestart()
    {
        Time.timeScale = 1;
        GameRetry();
    }

    // [추가] 게임 종료
    public void OnClickQuit()
    {
        GameExit();
    }

    void UpdateUI()
    {
        if (monthText) monthText.text = $"{currentMonth}개월차";
        if (cashText) cashText.text = $"{cash:N0}";
        if (savingsText) savingsText.text = $"{savings:N0}";
        if (loanText) loanText.text = $"{loan:N0}";
        if (stressText) { stressText.text = $"{stress}%"; }
        if (salaryText) salaryText.text = $"{baseSalary:N0}";
        if (jobLevelText) jobLevelText.text = $"Lv. {jobLevel}";

        if (isSavingsJoined)
        {
            if (savingsBtnText) savingsBtnText.text = "가입 완료";
            if (savingsJoinBtn) savingsJoinBtn.interactable = false;
        }
    }

    void EndGame(string type)
    {
        isGameOver = true;
        Time.timeScale = 0;

        if (fadeImage != null) { fadeImage.color = new Color(0, 0, 0, 0); fadeImage.gameObject.SetActive(false); }
        if (tipText != null) tipText.gameObject.SetActive(false);

        CloseAllPanels();

        if (SoundManager.instance)
        {
            SoundManager.instance.StopBGM();

            if (type == "스트레스") SoundManager.instance.PlaySFX(SoundManager.instance.failStressBgm);
            else if (type == "파산") SoundManager.instance.PlaySFX(SoundManager.instance.failMoneyBgm);
            else SoundManager.instance.PlaySFX(SoundManager.instance.successBgm);
        }

        if (type == "스트레스")
        {
            failStressPanel.SetActive(true);
        }
        else if (type == "파산")
        {
            UpdateUI(); failMoneyPanel.SetActive(true);
        }
        else
        {
            successPanel.SetActive(true);
            if (txtResultScore != null) txtResultScore.text = $" {(cash + savings - loan):N0}";
        }
    }
}