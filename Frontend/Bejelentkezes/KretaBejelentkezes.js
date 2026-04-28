const API_BASE_URL = "https://localhost:7273/api/User";

const loginForm = document.getElementById("login-form");
const loginMessage = document.getElementById("login-message");

function showMessage(element, message, isSuccess = false) {
    element.textContent = message;
    element.className = isSuccess
        ? "mt-3 alert alert-success"
        : "mt-3 alert alert-danger";
}

function clearMessage(element) {
    element.textContent = "";
    element.className = "mt-3";
}

function redirectByRole(user) {
    const role = (user.role || user.Role || "").toString().trim().toLowerCase();

    if (role === "diak") {
        window.location.href = "../KretaDiakFelulet/Diak.html";
    }
    else if (role === "tanar") {
        window.location.href = "../KretaTanarFelulet/Tanar.html";
    }
    else if (role === "admin") {
        window.location.href = "../KretaAdminFelulet/Admin.html";
    }
    else {
        showMessage(loginMessage, "Sikertelen");
      
    }
}

if (loginForm) {
    loginForm.addEventListener("submit", async function (e) {
        e.preventDefault();
        clearMessage(loginMessage);
        const username = loginForm.username.value.trim();
        const password = loginForm.password.value.trim();
        if (!username || !password) {
            showMessage(loginMessage, "Kérlek tölts ki minden mezőt.");
            return;
        }
        try {
            const url = `${API_BASE_URL}/login?username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}`;
            const response = await fetch(url, {
                method: "POST",
                credentials: "include"
            });
            if (response.ok) {
                const user = await response.json();
               
                localStorage.setItem("kretaUser", JSON.stringify(user));
                showMessage(loginMessage, "Sikeres bejelentkezés.", true);
                setTimeout(() => {
                    redirectByRole(user);
                }, 500);
                return;
            }
            if (response.status === 401) {
                showMessage(loginMessage, "Hibás felhasználónév vagy jelszó.");
                return;
            }
            const errorText = await response.text();
            showMessage(loginMessage, errorText || "Sikertelen bejelentkezés.");
        } catch (error) {
        
            showMessage(loginMessage, "Nem sikerült kapcsolódni a szerverhez.");
        }
    });
}