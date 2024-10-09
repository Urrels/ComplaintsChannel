import { Component, EventEmitter ,Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplaintDataService } from '../../services/complaint-data.service';


@Component({
  selector: 'app-misdatos',
  standalone: true,
  imports: [CommonModule,FormsModule],
  templateUrl: './misdatos.component.html',
  styleUrl: './misdatos.component.css'
})
export class MisdatosComponent {
  nombre: string = '';
  apellido: string = '';
  estado: string = '';
  cargo: string = '';
  area: string = '';
  sexo: string = ''; 
  contacto: string = '';
  rut: string = '';
  eCompanyStatus: number = 1;  // Valor por defecto

  @Output() cerrar = new EventEmitter<void>();

  constructor(private complaintDataService: ComplaintDataService) {}

  guardarDatos() {
    // Guardar datos del denunciante en el servicio
    this.complaintDataService.setComplaintData({
      complainant: {
        names: this.nombre,
        lastName: this.apellido,
        eCompanyStatus: this.eCompanyStatus,  // Guardar el eCompanyStatus
        position: this.cargo,
        area: this.area,
        eGenre: this.sexo === 'Masculino' ? 1 : 2,  // Manejar el género
        contactPhone: this.contacto,
        rut: this.rut
      }
    });

    // Log para verificar que los datos se guardan correctamente
    console.log('Datos del denunciante guardados:', {
      names: this.nombre,
      lastName: this.apellido,
      position: this.cargo,
      area: this.area,
      eCompanyStatus: this.eCompanyStatus,
      contactPhone: this.contacto,
      rut: this.rut,
      eGenre: this.sexo,
    });

    this.cerrar.emit();  // Cerrar el formulario después de guardar los datos
  }
}
