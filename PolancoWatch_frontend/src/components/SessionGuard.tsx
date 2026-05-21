import { useEffect, useState } from 'react';
import { jwtDecode } from 'jwt-decode';
import { useNavigate } from 'react-router-dom';
import { authService } from '../services/api';
import { ShieldAlert } from 'lucide-react';

export default function SessionGuard() {
  const [warningMessage, setWarningMessage] = useState<string | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    // Check every minute
    const interval = setInterval(() => {
      const token = localStorage.getItem('token');
      if (!token) return;

      try {
        const decoded = jwtDecode<{exp: number}>(token);
        const currentTime = Date.now();
        const expirationTime = decoded.exp * 1000;
        const timeLeftMs = expirationTime - currentTime;
        const timeLeftMinutes = Math.floor(timeLeftMs / 60000);

        if (timeLeftMs <= 0) {
          // Token expired right now during check
          authService.logout();
          navigate('/login');
        } else if (timeLeftMinutes <= 5) {
          // Warning: Less than 5 minutes
          setWarningMessage(`Tu sesión de administrador expirará en ${timeLeftMinutes} minuto${timeLeftMinutes !== 1 ? 's' : ''}. Por favor, guarda tus cambios.`);
        } else {
          setWarningMessage(null);
        }
      } catch (e) {
        authService.logout();
        navigate('/login');
      }
    }, 60000);

    return () => clearInterval(interval);
  }, [navigate]);

  if (!warningMessage) return null;

  return (
    <div className="fixed bottom-6 right-6 z-[200] max-w-sm animate-fade-in">
      <div className="bg-obsidian-900 border border-amber-500/30 p-4 rounded-xl shadow-[0_0_30px_rgba(245,158,11,0.15)] flex gap-4 items-start relative overflow-hidden group">
        <div className="absolute inset-0 bg-linear-to-r from-amber-500/10 to-transparent pointer-events-none" />
        
        <div className="flex-shrink-0 pt-0.5 relative z-10">
          <ShieldAlert className="text-amber-400 animate-pulse" size={24} />
        </div>
        <div className="relative z-10">
          <h4 className="text-amber-400 font-bold text-sm tracking-widest uppercase mb-1 drop-shadow-[0_0_8px_rgba(245,158,11,0.5)]">
            Alerta de Sesión
          </h4>
          <p className="text-slate-300 text-xs font-medium leading-relaxed">
            {warningMessage}
          </p>
        </div>
      </div>
    </div>
  );
}
