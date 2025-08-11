
let knowledge = {};
let isLoading = false;
let currentTheme = 'light';



// Smart suggestions based on context
const smartSuggestions = {
  loan_types: [
    "Business loan details",
    "Gold loan requirements", 
    "CD loan interest rates",
    "Personal loan process"
  ],
  application: [
    "Required documents",
    "Eligibility criteria",
    "Application process",
    "Processing time"
  ],
  documents: [
    "ID proof requirements",
    "Income proof documents",
    "Address proof needed",
    "PAN card mandatory?"
  ]
};

fetch('../js/chat/knowledge.json')
  .then(res => res.json())
  .then(data => {
    knowledge = data; // Correct
    addBotMessage("Hello! I'm the Yogloans assistant. How can I help you today with your loan queries?");
  })
  .catch(error => {
    console.error('Error loading knowledge:', error);
    addBotMessage("Hello! I'm the Yogloans assistant. How can I help you today with your loan queries?");
  });

function findBestResponse(input) {
  input = input.toLowerCase().trim();
  let bestIntent = null;
  let maxScore = 0;

  const inputWords = input.split(/\s+/);

  if (!knowledge.intents) {
    console.error('Knowledge data not loaded properly');
    return "Oops! Something went wrong. Please try again later.";
  }

  for (const intent of knowledge.intents) {
    let score = 0;

    const sortedKeywords = intent.keywords.sort((a, b) => b.length - a.length);
    for (const keyword of sortedKeywords) {
      const keywordLower = keyword.toLowerCase();

      if (input === keywordLower) {
        score += 10;
      } else if (input.includes(keywordLower)) {
        score += 3;
      }

      const keywordWords = keywordLower.split(/\s+/);
      for (const inputWord of inputWords) {
        for (const keywordWord of keywordWords) {
          if (inputWord === keywordWord) {
            score += 2;
          } else if (
            inputWord.length >= 4 &&
            keywordWord.length >= 4 &&
            (inputWord.includes(keywordWord) || keywordWord.includes(inputWord))
          ) {
            score += 1;
          }
        }
      }
    }

    if (score > maxScore) {
      maxScore = score;
      bestIntent = intent;
    }
  }

  if (maxScore >= 4 && bestIntent) {
    return bestIntent.response;
  } else {
    return "I'm sorry, I couldn't understand that. Please ask a question related to loans, documents, or application process.";
  }
}




function addUserMessage(text) {
  const msgBox = document.getElementById("messages");
  const messageDiv = document.createElement("div");
  messageDiv.className = "message user";
  messageDiv.innerHTML = `
    <div class="user-message">${text}</div>
    <div class="user-avatar">U</div>
  `;
  msgBox.appendChild(messageDiv);
  msgBox.scrollTop = msgBox.scrollHeight;
}

function addBotMessage(text) {
  const msgBox = document.getElementById("messages");
  const messageDiv = document.createElement("div");
  messageDiv.className = "message bot";
  
  // Add reaction buttons
  const reactionsHTML = `
    <div class="message-reactions">
      <button class="reaction-btn" onclick="addReaction(this, '👍')" title="Helpful">👍</button>
      <button class="reaction-btn" onclick="addReaction(this, '👎')" title="Not helpful">👎</button>
      <button class="reaction-btn" onclick="addReaction(this, '💬')" title="Need more info">💬</button>
    </div>
  `;

  messageDiv.innerHTML = `
    <div class="bot-avatar">Y</div>
    <div class="bot-message">
      ${text}
      ${reactionsHTML}
    </div>
  `;
  
  msgBox.appendChild(messageDiv);
  msgBox.scrollTop = msgBox.scrollHeight;
  
  // Add hover effect to buttons in bot messages
  const buttons = messageDiv.querySelectorAll('button');
  buttons.forEach(button => {
    button.addEventListener('mouseenter', function() {
      this.style.transform = 'translateY(-2px)';
      this.style.boxShadow = '0 5px 15px rgba(102, 126, 234, 0.4)';
    });
    button.addEventListener('mouseleave', function() {
      this.style.transform = 'translateY(0)';
      this.style.boxShadow = 'none';
    });
  });
}



function addReaction(button, emoji) {
  // Remove existing reactions
  const reactions = button.parentElement.querySelectorAll('.reaction-btn');
  reactions.forEach(btn => btn.style.background = 'none');
  
  // Add reaction
  button.style.background = 'rgba(102, 126, 234, 0.2)';
  button.style.borderRadius = '50%';
  
  // Show feedback
  showFeedback(emoji === '👍' ? 'Thank you for the feedback!' : 'We\'ll improve our response.');
}

function showFeedback(message) {
  const feedbackDiv = document.createElement('div');
  feedbackDiv.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    background: var(--primary-gradient);
    color: white;
    padding: 10px 20px;
    border-radius: 20px;
    z-index: 1000;
    animation: slideIn 0.3s ease;
  `;
  feedbackDiv.textContent = message;
  document.body.appendChild(feedbackDiv);
  
  setTimeout(() => {
    feedbackDiv.remove();
  }, 3000);
}

function showTypingIndicator() {
  const msgBox = document.getElementById("messages");
  const typingDiv = document.createElement("div");
  typingDiv.className = "message bot typing-indicator";
  typingDiv.id = "typing-indicator";
  typingDiv.style.display = "flex";
  typingDiv.innerHTML = `
    <div class="bot-avatar">Y</div>
    <div class="bot-message">
      <div class="typing-dots">
        <div class="typing-dot"></div>
        <div class="typing-dot"></div>
        <div class="typing-dot"></div>
      </div>
    </div>
  `;
  msgBox.appendChild(typingDiv);
  msgBox.scrollTop = msgBox.scrollHeight;
}

function hideTypingIndicator() {
  const typingIndicator = document.getElementById("typing-indicator");
  if (typingIndicator) {
    typingIndicator.remove();
  }
}

function showLoader() {
  isLoading = true;
  const msgBox = document.getElementById("messages");
  const loaderDiv = document.createElement("div");
  loaderDiv.className = "loader-container";
  loaderDiv.id = "loader";
  loaderDiv.innerHTML = `
    <div class="bot-avatar">Y</div>
    <div class="bot-message">
      <div class="loader">
        <div class="loader-dot"></div>
        <div class="loader-dot"></div>
        <div class="loader-dot"></div>
      </div>
      <div class="loader-text">Processing your request...</div>
    </div>
  `;
  msgBox.appendChild(loaderDiv);
  msgBox.scrollTop = msgBox.scrollHeight;
}

function hideLoader() {
  isLoading = false;
  const loader = document.getElementById("loader");
  if (loader) {
    loader.remove();
  }
}

function toggleTheme() {
  const body = document.body;
  const themeToggle = document.querySelector('.theme-toggle');
  
  if (currentTheme === 'light') {
    body.setAttribute('data-theme', 'dark');
    currentTheme = 'dark';
    themeToggle.innerHTML = `
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="12" cy="12" r="5"></circle>
        <path d="M12 1v2M12 21v2M4.22 4.22l1.42 1.42M18.36 18.36l1.42 1.42M1 12h2M21 12h2M4.22 19.78l1.42-1.42M18.36 5.64l1.42-1.42"></path>
      </svg>
    `;
  } else {
    body.removeAttribute('data-theme');
    currentTheme = 'light';
    themeToggle.innerHTML = `
      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
        <circle cx="12" cy="12" r="5"></circle>
        <line x1="12" y1="1" x2="12" y2="3"></line>
        <line x1="12" y1="21" x2="12" y2="23"></line>
        <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"></line>
        <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"></line>
        <line x1="1" y1="12" x2="3" y2="12"></line>
        <line x1="21" y1="12" x2="23" y2="12"></line>
        <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"></line>
        <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"></line>
      </svg>
    `;
  }
  
  // Save theme preference
  localStorage.setItem('chatbot-theme', currentTheme);
}

// Handle Enter key press
document.addEventListener('DOMContentLoaded', function() {
  const inputBox = document.getElementById("userInput");
  const sendBtn = document.getElementById("sendBtn");
  
  inputBox.addEventListener('keypress', function(e) {
    if (e.key === 'Enter') {
      handleUserMessage();
    }
  });
  
  // Focus on input when page loads
  inputBox.focus();
  
  // Add click event for send button
  sendBtn.addEventListener('click', handleUserMessage);
  
  // Load saved theme
  const savedTheme = localStorage.getItem('chatbot-theme');
  if (savedTheme === 'dark') {
    toggleTheme();
  }
});

// Auto-scroll to bottom when new messages are added
function scrollToBottom() {
  const msgBox = document.getElementById("messages");
  msgBox.scrollTop = msgBox.scrollHeight;
}

// Add smooth scrolling
function smoothScrollToBottom() {
  const msgBox = document.getElementById("messages");
  msgBox.scrollTo({
    top: msgBox.scrollHeight,
    behavior: 'smooth'
  });
}

// Add CSS animation for feedback
const style = document.createElement('style');
style.textContent = `
  @keyframes slideIn {
    from {
      transform: translateX(100%);
      opacity: 0;
    }
    to {
      transform: translateX(0);
      opacity: 1;
    }
  }
`;
document.head.appendChild(style);
