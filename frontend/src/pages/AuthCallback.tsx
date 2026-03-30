import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useMsal } from '@azure/msal-react';
import { authService } from '../services/api';
import { useAuthStore } from '../store/authStore';

function AuthCallback() {
  const navigate = useNavigate();
  const { instance, accounts } = useMsal();
  const setAuth = useAuthStore((state) => state.setAuth);

  useEffect(() => {
    const handleCallback = async () => {
      try {
        if (accounts.length > 0) {
          const response = await instance.acquireTokenSilent({
            scopes: ['User.Read'],
            account: accounts[0],
          });

          const redirectUri = import.meta.env.VITE_REDIRECT_URI || 'http://localhost:5173/auth/callback';
          
          const tokenResponse = await authService.token(
            response.accessToken,
            redirectUri
          );

          setAuth(tokenResponse.token, tokenResponse.usuario);
          navigate('/dashboard');
        }
      } catch (error) {
        console.error('Callback error:', error);
        navigate('/login');
      }
    };

    handleCallback();
  }, [accounts, instance, navigate, setAuth]);

  return (
    <div className="loading">
      <div className="spinner"></div>
    </div>
  );
}

export default AuthCallback;
