import { useEffect } from 'react';
import { useMsal } from '@azure/msal-react';
import { loginRequest } from '../services/msal';

function Login() {
  const { instance } = useMsal();

  useEffect(() => {
    instance.initialize().then(() => {
      instance.loginPopup(loginRequest).catch((error) => {
        console.error('Login failed:', error);
      });
    });
  }, [instance]);

  const handleLogin = async () => {
    try {
      await instance.initialize();
      instance.loginPopup(loginRequest);
    } catch (error) {
      console.error('Login error:', error);
    }
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-logo">🎓</div>
        <h1 className="login-title">ESPOCH</h1>
        <p className="login-subtitle">Plataforma de Experiencia del Colaborador</p>
        <button className="btn btn-primary" onClick={handleLogin} style={{ width: '100%' }}>
          Iniciar Sesión con Microsoft
        </button>
      </div>
    </div>
  );
}

export default Login;
